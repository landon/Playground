using Algorithms.FixerBreaker.Chronicle;
using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim
{
    public class SlimMind : IMind
    {
        public bool StopAnalysisOnBreakerWin { get; set; }
        public bool NearlyColorableOnly { get; set; }
        public bool SuperabundantOnly { get; set; }
        public int MinPot { get; set; }
        public int MaxPot { get; set; }
        public Board BreakerWonBoard { get; private set; }

        public bool FixerWonAllNearlyColorableBoards { get; set; }
        public int TotalPositions { get; private set; }

        Knowledge Knowledge { get; set; }
        ColoringAnalyzer ColoringAnalyzer {get; set;}
        SlimSwapAnalyzer SwapAnalyzer { get; set; }
        Dictionary<int, Board> BoardLookup { get; set; }
        Dictionary<Board, int> BoardIDLookup { get; set; }
        List<int> RemainingBoardIDs { get; set; }
        SortedIntList WonBoardIDs { get; set; }
        int NextBoardID { get; set; }

        public SlimMind(Graph g)
        {
            SuperabundantOnly = false;
            NearlyColorableOnly = false;
            MaxPot = MetaKnowledge.Infinity;
            Knowledge = new Knowledge(g);

            ColoringAnalyzer = new ColoringAnalyzer();
        }

        public bool Analyze(Template template, Action<Tuple<string, int>> progress = null)
        {
            BoardLookup = new Dictionary<int, Board>();
            BoardIDLookup = new Dictionary<Board, int>();
            WonBoardIDs = new SortedIntList();
            SwapAnalyzer = new SlimSwapAnalyzer(Knowledge, BoardLookup, BoardIDLookup);

            FixerWonAllNearlyColorableBoards = true;

            var minimumColorCount = Math.Max(MinPot, template.Sizes.Max());
            var maximumColorCount = Math.Min(MaxPot, template.Sizes.Sum());

            var foundAtLeastOneBoard = false;
            foreach (var colorCount in MetaKnowledge.Interval(minimumColorCount, maximumColorCount))
            {
                GenerateAllBoards(template, colorCount, progress);
                if (foundAtLeastOneBoard && BoardLookup.Count <= 0)
                    break;

                TotalPositions = BoardLookup.Count;
                foundAtLeastOneBoard = true;
                RemainingBoardIDs = BoardLookup.Keys.ToList();

                var breakerWin = !Analyze(progress);
                if (breakerWin && StopAnalysisOnBreakerWin)
                    return false;

                Knowledge[template].Promote(colorCount);
            }

            return Knowledge[template].LostBoards.Count() <= 0;
        }

        bool Analyze(Action<Tuple<string, int>> progress = null)
        {
            int winLength = 0;

            var totalBoards = RemainingBoardIDs.Count;
            var lastP = -1;

            for (int i = RemainingBoardIDs.Count - 1; i >= 0; i--)
            {
                var id = RemainingBoardIDs[i];

                var b = BoardLookup[id];
                if (ColoringAnalyzer.Analyze(Knowledge, b))
                {
                    RemainingBoardIDs.RemoveAt(i);
                    WonBoardIDs.Add(id);

                    if (progress != null)
                    {
                        var p = 100 * (totalBoards - RemainingBoardIDs.Count) / totalBoards;
                        if (p > lastP)
                        {
                            progress(new Tuple<string, int>("Finding all colorable positions...", p));
                            lastP = p;
                        }
                    }
                }
            }

            while (RemainingBoardIDs.Count > 0)
            {
                winLength++;

                var count = RemainingBoardIDs.Count;

                for (int i = RemainingBoardIDs.Count - 1; i >= 0; i--)
                {
                    var id = RemainingBoardIDs[i];

                    var b = BoardLookup[id];
                    if (SwapAnalyzer.Analyze(id, WonBoardIDs))
                    {
                        RemainingBoardIDs.RemoveAt(i);
                        WonBoardIDs.Add(id);

                        if (progress != null)
                        {
                            var p = 100 * (totalBoards - RemainingBoardIDs.Count) / totalBoards;
                            if (p > lastP)
                            {
                                progress(new Tuple<string, int>(string.Format("Finding all {0} move wins...", winLength), p));
                                lastP = p;
                            }
                        }
                    }
                }

                if (RemainingBoardIDs.Count == count)
                {
                    foreach (var id in RemainingBoardIDs)
                    {
                        var b = BoardLookup[id];
                        Knowledge[b.Template.Value][b.ColorCount].AddLoss(b);
                    }

                    var g = Knowledge.GraphKnowledge.Graph;

                    var nearlyColorable = Enumerable.Range(0, Knowledge.GraphKnowledge.LineGraph.N).All(e => RemainingBoardIDs.Any(id => ColoringAnalyzer.ColorableWithoutEdge(Knowledge, BoardLookup[id], e)));
                    if (nearlyColorable)
                    {
                        FixerWonAllNearlyColorableBoards = false;
                        BreakerWonBoard = RemainingBoardIDs.Where(id => 
                            {
                                var b = BoardLookup[id];
                                return g.DegreeCondition(b) && ColoringAnalyzer.ColorableWithoutEdge(Knowledge, b, 0);
                            }).Select(id => BoardLookup[id])
                              .FirstOrDefault();

                        if (BreakerWonBoard == null)
                            BreakerWonBoard = BoardLookup[RemainingBoardIDs.First(id => ColoringAnalyzer.ColorableWithoutEdge(Knowledge, BoardLookup[id], 0))];
                    }

                    return false;
                }
            }

            return true;
        }

        void GenerateAllBoards(Template template, int colorCount, Action<Tuple<string, int>> progress = null)
        {
            var boards = new HashSet<Board>();

            var vertices = Enumerable.Range(0, template.Sizes.Count).ToList();
            var potSet = Enumerable.Range(0, colorCount).ToList();
            var pot = potSet.ToInt64();

            var fix = vertices.OrderBy(v => Math.Abs(template.Sizes[v] - colorCount / 2)).First();
            var lastP = -1;
            var current = 0;
            var total = vertices.Aggregate(1L, (x, v) => 
                {
                    if (v == fix)
                        return x;

                    return x * ListUtility.BinomialCoefficient(potSet.Count, template.Sizes[v]);
                });

            foreach (var assignmentSets in vertices.Select(v =>
            {
                if (v == fix)
                    return potSet.Take(template.Sizes[v]).ToList().EnList();

                return ListUtility.EnumerateSublists(potSet, template.Sizes[v]);
            }).CartesianProduct())
            {
                var sets = assignmentSets.ToList();
                var totalColors = sets.SelectMany(set => set).Distinct().Count();
                if (totalColors < colorCount)
                    continue;

                var stacks = sets.Select(list => list.ToInt64()).ToList();

                if (SuperabundantOnly && !Knowledge.GraphKnowledge.Graph.DegreeCondition(stacks, pot))
                    continue;

                var board = new Board(stacks, pot);

                var count = boards.Count;
                boards.Add(board);

                if (boards.Count > count)
                {
                    BoardLookup[NextBoardID] = board;
                    BoardIDLookup[board] = NextBoardID;

                    NextBoardID++;
                }

                if (progress != null)
                {
                    var p = (int)(100 * current / total);
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>("Finding all positions...", p));
                        lastP = p;
                    }
                }

                current++;
            }
        }
    }
}
