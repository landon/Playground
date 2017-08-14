using Algorithms.FixerBreaker.Chronicle;
using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class Mind
    {
        public bool StopAnalysisOnBreakerWin { get; set; }
        public bool FindCleanestWin { get; set; }
        public bool NearlyColorableOnly { get; set; }
        public bool SuperabundantOnly { get; set; }
        public int MinPot { get; set; }
        public int MaxPot { get; set; }
        public Board BreakerWonBoard { get; private set; }

        public bool FixerWonAllNearlyColorableBoards { get; set; }
        public int TotalPositions { get; private set; }

        List<IBoardAnalyzer> BoardAnalysisPipeline { get; set; }
        Knowledge Knowledge { get; set; }

        public Mind(Graph g)
        {
            FindCleanestWin = true;
            SuperabundantOnly = true;
            NearlyColorableOnly = false;
            MaxPot = MetaKnowledge.Infinity;
            Knowledge = new Knowledge(g);
            
            BoardAnalysisPipeline = new List<IBoardAnalyzer>();
            BoardAnalysisPipeline.Add(new ColoringAnalyzer());
        }

        public void AddBoardAnalyzer(IBoardAnalyzer analyzer)
        {
            BoardAnalysisPipeline.Add(analyzer);
        }

        public bool Analyze(Template template, Action<ThoughtProgress> progress = null)
        {
            TotalPositions = 0;
            FixerWonAllNearlyColorableBoards = true;

            EnsureSwapAnalyzerExistence();

            var minimumColorCount = Math.Max(MinPot, template.Sizes.Max());
            var maximumColorCount = Math.Min(MaxPot, template.Sizes.Sum());

            var foundAtLeastOneBoard = false;
            foreach (var colorCount in MetaKnowledge.Interval(minimumColorCount, maximumColorCount))
            {
                if (progress != null)
                    progress(new ThoughtProgress() { IsInitialThought = true });

                var boards = GenerateAllBoards(template, colorCount, progress).ToList();
                if (foundAtLeastOneBoard && boards.Count <= 0)
                    break;

                TotalPositions += boards.Count;
                foundAtLeastOneBoard = true;

                var breakerWin = !Analyze(boards, progress);
                if (breakerWin && StopAnalysisOnBreakerWin)
                    return false;

                Knowledge[template].Promote(colorCount);
            }

            return Knowledge[template].LostBoards.Count() <= 0;
        }

        bool Analyze(List<Board> boards, Action<ThoughtProgress> progress = null)
        {
            int winLength = 0;

            foreach (var analyzer in BoardAnalysisPipeline.Where(a => !a.IsKnowledgeDependent))
            {
                var chunk = new List<Board>();
                boards.RemoveAll(b => analyzer.Analyze(Knowledge, b), (removed) =>
                {
                    chunk.Add(removed);

                    if (chunk.Count > 100)
                    {
                        if (progress != null)
                            progress(new ThoughtProgress() { BoardsRemoved = chunk.ToList(), WinLength = winLength });
                        
                        chunk.Clear();
                    }
                });

                if (chunk.Count > 0)
                {
                    if (progress != null)
                        progress(new ThoughtProgress() { BoardsRemoved = chunk.ToList(), WinLength = winLength });
                }
            }

            var knowledgeDependentAnalyzers = BoardAnalysisPipeline.Where(a => a.IsKnowledgeDependent).ToList();

            while (boards.Count > 0)
            {
                winLength++;

                var count = boards.Count;

                foreach (var analyzer in knowledgeDependentAnalyzers)
                {
                    boards.RemoveAll(b => analyzer.Analyze(Knowledge, b), (removed) =>
                        {
                            if (progress != null)
                                progress(new ThoughtProgress() { BoardsRemoved = removed.EnList(), WinLength = winLength });
                        });
                }

                if (boards.Count == count)
                {
                    foreach (var b in boards)
                        Knowledge[b.Template.Value][b.ColorCount].AddLoss(b);

                    var g = Knowledge.GraphKnowledge.Graph;

                    var nearlyColorable = Enumerable.Range(0, Knowledge.GraphKnowledge.LineGraph.N).All(e => boards.Any(b => ColoringAnalyzer.ColorableWithoutEdge(Knowledge, b, e)));
                    if (nearlyColorable)
                    {
                        FixerWonAllNearlyColorableBoards = false;
                        BreakerWonBoard = boards.FirstOrDefault(b => g.DegreeCondition(b) && ColoringAnalyzer.ColorableWithoutEdge(Knowledge, b, 0));
                        if (BreakerWonBoard == null)
                            BreakerWonBoard = boards.First(b => ColoringAnalyzer.ColorableWithoutEdge(Knowledge, b, 0));
                    }

                    return false;
                }
            }

            return true;
        }

        void EnsureSwapAnalyzerExistence()
        {
            if (!BoardAnalysisPipeline.Any(a => a is SwapAnalyzer))
                BoardAnalysisPipeline.Add(new SwapAnalyzer(FindCleanestWin, false));
        }

        HashSet<Board> GenerateAllBoards(Template template, int colorCount, Action<ThoughtProgress> progress = null)
        {
            var boards = new HashSet<Board>();

            var vertices = Enumerable.Range(0, template.Sizes.Count).ToList();
            var potSet = Enumerable.Range(0, colorCount).ToList();
            var pot = potSet.ToInt64();

            var fix = vertices.OrderBy(v => Math.Abs(template.Sizes[v] - colorCount / 2)).First();

            var boardsChunk = new List<Board>();
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

                var superAbundant = Knowledge.GraphKnowledge.Graph.DegreeCondition(stacks, pot);
                var board = new Board(stacks, pot);

                if (SuperabundantOnly && !superAbundant)
                    continue;

                boards.Add(board);
                boardsChunk.Add(board);

                if (boardsChunk.Count > 100)
                {
                    if (progress != null)
                        progress(new ThoughtProgress() { BoardsAdded = boardsChunk.ToList(), WinLength = -1 });

                    boardsChunk.Clear();
                }
            }

            if (boardsChunk.Count > 0 && progress != null)
                progress(new ThoughtProgress() { BoardsAdded = boardsChunk.ToList(), WinLength = -1 });

            return boards;
        }

        static int TreeNodeNumber = 1;
        public IEnumerable<KnowledgeTree> EnumerateKnowledgeTrees(Template template)
        {
            var templateKnowledge = Knowledge[template];
            var colorCount = MetaKnowledge.Naturals().First(n => templateKnowledge.KnowledgeExists(n) && !templateKnowledge.KnowledgeExists(n + 1));

            return templateKnowledge[colorCount].EnumerateBoardKnowledge().Select(kvp => GenerateKnowledgeTree(kvp.Key));
        }
        public KnowledgeTree GenerateKnowledgeTree(Board board)
        {
            TreeNodeNumber = 1;
            var seenCache = new Dictionary<Board, Tuple<int, Board>>();
            var knowledgeTree = new KnowledgeTree();
            FillKnowledgeTree(knowledgeTree, board, seenCache);

            return knowledgeTree;
        }
        void FillKnowledgeTree(KnowledgeTree knowledgeTree, Board board, Dictionary<Board, Tuple<int, Board>> seenCache)
        {
            knowledgeTree.Board = board.ToString();
            knowledgeTree.Number = TreeNodeNumber++;

            var knowledge = Knowledge[board.Template.Value][board.ColorCount][board];
            if (knowledge.Exists())
            {
                if (knowledge.Reason != "good swap")
                {
                    knowledgeTree.Note = knowledge.Reason;
                }
                else
                {
                    Tuple<int, Board> seenBoard;
                    if (seenCache.TryGetValue(board, out seenBoard))
                    {
                        knowledgeTree.Note = "same as #" + seenBoard.Item1;
                        return;
                    }

                    var f = knowledge.Board.FindPermutation(board);
                    var colors = Board.ApplyMapping(f, new[] { knowledge.ColorPair.Item1, knowledge.ColorPair.Item2 }).ToList();
                    var colorPair = new Tuple<int, int>(colors[0], colors[1]);

                    foreach (var swap in knowledge.Swaps)
                    {
                        var tempBoard = board.Clone();
                        var moves = swap.Item2.Select(m => MetaKnowledge.MapMove(f, m)).ToList();
                        tempBoard.DoMoveCombination(moves);

                        var childKnowledgeTree = knowledgeTree.AddChild(swap);
                        childKnowledgeTree.ColorPair = colorPair;
                        FillKnowledgeTree(childKnowledgeTree, tempBoard, seenCache);
                    }
                }
            }
            else
            {
                throw new Exception("something went wrong in building search tree");
            }

            seenCache[board] = new Tuple<int, Board>(knowledgeTree.Number, board);
        }

        public Tuple<Graph, Dictionary<int, KeyValuePair<Board, BoardKnowledge>>, Dictionary<Tuple<int, int>, Tuple<List<List<int>>, List<Move>, string>>> GenerateTemplateKnowledgeGraph(Template template)
        {
            var templateKnowledge = Knowledge[template];
            var colorCount = MetaKnowledge.Naturals().First(n => templateKnowledge.KnowledgeExists(n) && !templateKnowledge.KnowledgeExists(n + 1));

            var nextVertexID = 0;
            var vertexLookup = new Dictionary<Board, int>();
            var swapLookup = new Dictionary<Tuple<int, int>, Tuple<List<List<int>>, List<Move>, string>>();
            var boardLookup = new Dictionary<int, KeyValuePair<Board, BoardKnowledge>>();
            var outEdges = new Dictionary<int, List<int>>();

            foreach (var kvp in templateKnowledge[colorCount].EnumerateBoardKnowledge())
            {
                vertexLookup[kvp.Key] = nextVertexID;
                boardLookup[nextVertexID] = kvp;
                outEdges[nextVertexID] = new List<int>();
                nextVertexID++;
            }

            foreach (var b in templateKnowledge.LostBoards)
            {
                vertexLookup[b] = nextVertexID;
                boardLookup[nextVertexID] = new KeyValuePair<Board, BoardKnowledge>(b, new BoardKnowledge(b, "breaker wins"));
                outEdges[nextVertexID] = new List<int>();
                nextVertexID++;
            }

            foreach (var kvp in templateKnowledge[colorCount].EnumerateBoardKnowledge())
            {
                if (kvp.Value.Reason == "good swap")
                {
                    var v = vertexLookup[kvp.Key];

                    var f = kvp.Value.Board.FindPermutation(kvp.Key);
                    var colors = Board.ApplyMapping(f, new[] { kvp.Value.ColorPair.Item1, kvp.Value.ColorPair.Item2 }).ToList();
                    var colorPair = new Tuple<int, int>(colors[0], colors[1]);

                    foreach (var swap in kvp.Value.Swaps)
                    {
                        var tempBoard = kvp.Key.Clone();
                        var moves = swap.Item2.Select(m => MetaKnowledge.MapMove(f, m)).ToList();
                        tempBoard.DoMoveCombination(moves);

                        var w = vertexLookup[tempBoard];
                        outEdges[v].Add(w);

                        swapLookup[new Tuple<int, int>(v, w)] = swap;
                    }
                }
            }

            var edgeWeights = new List<int>();
            for (int i = 0; i < nextVertexID; i++)
            {
                for (int j = i + 1; j < nextVertexID; j++)
                {
                    if (outEdges[i].Contains(j))
                        edgeWeights.Add(1);
                    else if (outEdges[j].Contains(i))
                        edgeWeights.Add(-1);
                    else
                        edgeWeights.Add(0);
                }
            }

            return new Tuple<Graph, Dictionary<int, KeyValuePair<Board, BoardKnowledge>>, Dictionary<Tuple<int, int>, Tuple<List<List<int>>, List<Move>, string>>>(new Graph(edgeWeights), boardLookup, swapLookup);
        }
    }
}
