using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimMind : IMind
    {
        List<SuperSlimBoard> _remainingBoards;
        HashSet<SuperSlimBoard> _wonBoards;
        SuperSlimSwapAnalyzer _swapAnalyzer;
        SuperSlimColoringAnalyzer _coloringAnalyzer;
        Graph _graph;
        Graph _lineGraph;
        public List<Tuple<int, int>> _edges;
        int _totalPositions;

        public int MinPot { get; set; }
        public int MaxPot { get; set; }
        public bool FixerWonAllNearlyColorableBoards { get; private set; }
        public bool HasNonSuperabundantBoardThatIsNearlyColorable { get; private set; }
        public int TotalPositions { get { return _totalPositions; } }
        public SuperSlimBoard BreakerWonBoard { get; private set; }
        public List<int> BoardCounts { get; private set; }
        public List<List<int>> BoardCountsList { get; private set; }
        public bool OnlyConsiderNearlyColorableBoards { get; set; }
        public bool ExcludeNonNearlyColorableNonSuperabundantBoards { get; set; }
        public int MissingEdgeIndex { get; set; }
        public bool SuperabundantOnly { get; set; }
        public bool ThinkHarder { get; set; }
        public bool PerformCompleteAnalysis { get; set; }

        public SuperSlimColoringAnalyzer ColoringAnalyzer { get { return _coloringAnalyzer; } }
        public List<SuperSlimBoard> NonColorableBoards { get; private set; }
        public List<SuperSlimBoard> ColorableBoards { get; private set; }
        public List<SuperSlimBoard> BreakerWonBoards { get; private set; }
        public HashSet<SuperSlimBoard> FixerWonBoards { get { return _wonBoards; } }
        public Dictionary<int, List<SuperSlimBoard>> BoardsOfDepth { get; private set; }
        public int ExtraPsi { get; set; }
        IWinFilter WinFilter { get; set; }

        public SuperSlimMind(Graph g, bool proofFindingMode = false, bool weaklyFixable = false)
        {
            _graph = g;
            BuildLineGraph();

            _coloringAnalyzer = new SuperSlimColoringAnalyzer(_lineGraph, GetEdgeColorList);
            _swapAnalyzer = new SuperSlimSwapAnalyzer(g.N, proofFindingMode, proofFindingMode || weaklyFixable);
            _wonBoards = new HashSet<SuperSlimBoard>();
            _remainingBoards = new List<SuperSlimBoard>();

            MissingEdgeIndex = -1;
        }

        public bool Analyze(Template template, Action<Tuple<string, int>> progress = null)
        {
            _wonBoards.Clear();
            _remainingBoards.Clear();
            BoardCountsList = new List<List<int>>();
            BreakerWonBoard = null;
            NonColorableBoards = new List<SuperSlimBoard>();
            ColorableBoards = new List<SuperSlimBoard>();
            BreakerWonBoards = new List<SuperSlimBoard>();

            if (WinFilter == null)
                WinFilter = new GreedyWinFilter(_swapAnalyzer);

            FixerWonAllNearlyColorableBoards = true;

            var minimumColorCount = Math.Max(MinPot, template.Sizes.Max());
            var maximumColorCount = Math.Min(MaxPot, template.Sizes.Sum());

            var foundAtLeastOneBoard = false;
            var fixerWin = true;
            for (int colorCount = minimumColorCount; colorCount <= maximumColorCount; colorCount++)
            {
                GenerateAllBoards(template, colorCount, progress);
                if (OnlyConsiderNearlyColorableBoards)
                {
                    if (MissingEdgeIndex >= 0)
                        _remainingBoards.RemoveAll(b => !NearlyColorableForEdge(b, MissingEdgeIndex), (x) => { });
                    else
                        _remainingBoards.RemoveAll(b => !NearlyColorableForSomeEdge(b), (x) => { });
                }

                if (foundAtLeastOneBoard && _remainingBoards.Count <= 0)
                    break;

                _totalPositions = _remainingBoards.Count + _wonBoards.Count;
                foundAtLeastOneBoard = true;

                fixerWin &= Analyze(progress);
            }

            return fixerWin;
        }

        bool Analyze(Action<Tuple<string, int>> progress = null)
        {
            int winLength = 0;
            var totalBoards = _remainingBoards.Count;
            var lastP = -1;

            BoardsOfDepth = new Dictionary<int, List<SuperSlimBoard>>();
            BoardCounts = new List<int>();
            BoardCountsList.Add(BoardCounts);
            BoardCounts.Add(_remainingBoards.Count);

            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                if (_coloringAnalyzer.Analyze(b))
                {
                    _remainingBoards.RemoveAt(i);
                    _wonBoards.Add(b);
                    ColorableBoards.Add(b);
                }

                if (progress != null)
                {
                    var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>("Finding all colorable positions...", p));
                        lastP = p;
                    }
                }
            }

            BoardsOfDepth[winLength] = _wonBoards.ToList();
            BoardCounts.Add(_remainingBoards.Count);

            var nonSuperabundantBoards = new List<SuperSlimBoard>();

            for (int i = _remainingBoards.Count - 1; i >= 0; i--)
            {
                var b = _remainingBoards[i];
                if (!IsSuperabundant(b))
                {
                    if (OnlyConsiderNearlyColorableBoards && MissingEdgeIndex >= 0)
                    {
                        HasNonSuperabundantBoardThatIsNearlyColorable = true;
                        BreakerWonBoard = b;

                        if (!PerformCompleteAnalysis)
                            return false;
                    }

                    _remainingBoards.RemoveAt(i);
                    nonSuperabundantBoards.Add(b);
                }

                if (progress != null)
                {
                    var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>("Finding all non-superabundant positions...", p));
                        lastP = p;
                    }
                }
            }

            if (nonSuperabundantBoards.Count > 0 && !SuperabundantOnly)
            {
                if (!OnlyConsiderNearlyColorableBoards && !ExcludeNonNearlyColorableNonSuperabundantBoards)
                {
                    FixerWonAllNearlyColorableBoards = false;
                    BreakerWonBoard = nonSuperabundantBoards[0];
                    BreakerWonBoards.AddRange(nonSuperabundantBoards);

                    if (!PerformCompleteAnalysis)
                        return false;
                }
                else if (ExistsNearlyColorableBoardForEachEdge(nonSuperabundantBoards))
                {
                    FixerWonAllNearlyColorableBoards = false;
                    HasNonSuperabundantBoardThatIsNearlyColorable = true;
                    BreakerWonBoard = nonSuperabundantBoards[0];
                    BreakerWonBoards.AddRange(nonSuperabundantBoards);
                    if (!PerformCompleteAnalysis)
                        return false;
                }
            }

            BoardCounts.Add(_remainingBoards.Count);
            NonColorableBoards.AddRange(_remainingBoards);

            while (_remainingBoards.Count > 0)
            {
                var count = _remainingBoards.Count;
                winLength++;

                var boardIndicesToAdd = WinFilter.Filter(_remainingBoards, _wonBoards).OrderByDescending(x => x).ToList();
                var wonBoards = _remainingBoards.Where((b, i) => boardIndicesToAdd.Contains(i)).ToList();
                foreach (var b in wonBoards)
                    _wonBoards.Add(b);
                BoardsOfDepth[winLength] = wonBoards;

                foreach (var i in boardIndicesToAdd)
                    _remainingBoards.RemoveAt(i);

                if (progress != null)
                {
                    var p = 100 * (totalBoards - _remainingBoards.Count) / totalBoards;
                    if (p > lastP)
                    {
                        progress(new Tuple<string, int>(string.Format("Finding all {0} move wins...", winLength), p));
                        lastP = p;
                    }
                }

                BoardCounts.Add(_remainingBoards.Count);

                if (_remainingBoards.Count == count)
                {
                    BreakerWonBoards.AddRange(_remainingBoards);

                    if (BreakerWonBoard == null)
                        BreakerWonBoard = _remainingBoards[0];

                    if (OnlyConsiderNearlyColorableBoards && MissingEdgeIndex >= 0)
                    {
                        FixerWonAllNearlyColorableBoards = false;
                    }
                    else if (ExistsNearlyColorableBoardForEachEdge(_remainingBoards))
                    {
                        FixerWonAllNearlyColorableBoards = false;
                        BreakerWonBoard = _remainingBoards.FirstOrDefault(b => _coloringAnalyzer.ColorableWithoutEdge(b, 0));

                        if (BreakerWonBoard == null)
                            BreakerWonBoard = _remainingBoards.First(b => _coloringAnalyzer.ColorableWithoutEdge(b, 0));
                    }

                    return false;
                }
            }

            return true;
        }

        bool NearlyColorableForEdge(SuperSlimBoard board, int edgeIndex)
        {
            return _coloringAnalyzer.ColorableWithoutEdge(board, edgeIndex);
        }

        bool ExistsNearlyColorableBoardForEachEdge(List<SuperSlimBoard> boards)
        {
            return Enumerable.Range(0, _lineGraph.N).All(e => boards.Any(b => NearlyColorableForEdge(b, e)));
        }

        public bool NearlyColorableForSomeEdge(SuperSlimBoard board)
        {
            return Enumerable.Range(0, _lineGraph.N).Any(e => NearlyColorableForEdge(board, e));
        }

        void LookAtSuperabundance()
        {
            var packs = Enumerable.Range(0, _lineGraph.N).Select(e => _remainingBoards.Where(b => _coloringAnalyzer.ColorableWithoutEdge(b, e)).ToList()).ToList();
            var goodPacks = packs.Where(pack => pack.All(ssb => IsSuperabundant(ssb))).ToList();
        }

        public bool IsSuperabundant(SuperSlimBoard b)
        {
            ulong subset = 0;

            int total = 0;
            while (subset < (1UL << b._stackCount))
            {
                total = 0;
                for (int i = 0; i < b._length; i++)
                    total += (subset & b._trace[i]).PopulationCount() / 2;

                var e = _graph.EdgesOn(subset.ToSet());
                if (total < e)
                    return false;

                subset++;
            }

            if (ExtraPsi > 0)
                return total >= _graph.E + ExtraPsi;

            return true;
        }

        public int ComputeAbundanceSurplus(SuperSlimBoard b)
        {
            int total = 0;
            for (int i = 0; i < b._length; i++)
                total += b._trace[i].PopulationCount() / 2;

            return total - _graph.E;
        }

        List<int> ComputeMatchingAbundanceShadow(SuperSlimBoard b)
        {
            var shadow = new List<int>();

            ulong subset = 0;
            while (subset < (1UL << b._stackCount))
            {
                int e;
                if (!IsMatchingAbundant(b, subset, out e))
                    shadow.Add(e);
                subset++;
            }

            shadow.Sort();

            return shadow;
        }

        bool IsMatchingAbundant(SuperSlimBoard b, ulong subset, out int e)
        {
            e = _graph.EdgesOn(subset.ToSet());

            int total = 0;
            for (int i = 0; i < b._length; i++)
            {
                var vc = (subset & b._trace[i]).ToSet();
                total += _lineGraph.IndependenceNumber(_graph.EdgeIndicesOn(vc));
            }

            return total >= e;
        }

        void GenerateAllBoards(Template template, int colorCount, Action<Tuple<string, int>> progress = null)
        {
            if (progress != null)
                progress(new Tuple<string, int>("Finding all positions...", 0));

            foreach (var t in BitLevelGeneration.Assignments_ulong.Generate(template.Sizes, colorCount))
            {
                var b = new SuperSlimBoard(t, template.Sizes.Count);
                _remainingBoards.Add(b);
            }
        }

        void BuildLineGraph()
        {
            var adjacent = _graph.Adjacent;
            int n = adjacent.GetUpperBound(0) + 1;

            _edges = new List<Tuple<int, int>>();
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (adjacent[i, j])
                        _edges.Add(new Tuple<int, int>(i, j));

            var meets = new bool[_edges.Count, _edges.Count];
            for (int i = 0; i < _edges.Count; i++)
                for (int j = i + 1; j < _edges.Count; j++)
                    if (_edges[i].Item1 == _edges[j].Item1 ||
                        _edges[i].Item1 == _edges[j].Item2 ||
                        _edges[i].Item2 == _edges[j].Item1 ||
                        _edges[i].Item2 == _edges[j].Item2)
                        meets[i, j] = meets[j, i] = true;

            _lineGraph = new Graph(meets);
        }

        long GetEdgeColorList(SuperSlimBoard b, int e)
        {
            var v1 = _edges[e].Item1;
            var v2 = _edges[e].Item2;
            var stacks = b.Stacks.Value;

            return stacks[v1] & stacks[v2];
        }

        public GameTreeInfo GetWinTreeInfo(SuperSlimBoard board)
        {
            return _swapAnalyzer.WinTreeInfo[board];
        }

        int _gameTreeIndex;
        public GameTree BuildGameTree(SuperSlimBoard board, bool win = true)
        {
            var seenBoards =  new Dictionary<SuperSlimBoard, int>();
            _gameTreeIndex = 1;
            return BuildGameTree(board, seenBoards, win);
        }

        GameTree BuildGameTree(SuperSlimBoard board, Dictionary<SuperSlimBoard, int> seenBoards, bool win = true, int depth = 0)
        {
            seenBoards[board] = _gameTreeIndex;
            var tree = new GameTree() { Board = board, IsFixerWin = win };
            tree.IsColorable = _coloringAnalyzer.Analyze(board);
            tree.IsSuperabundant = win || IsSuperabundant(board);
            tree.GameTreeIndex = _gameTreeIndex;
            _gameTreeIndex++;

            if (tree.IsColorable)
                return tree;

            if (!tree.IsSuperabundant)
                return tree;

            GameTreeInfo treeInfo;
            if (win)
                _swapAnalyzer.WinTreeInfo.TryGetValue(board, out treeInfo);
            else
                _swapAnalyzer.LossTreeInfo.TryGetValue(board, out treeInfo);

            if (treeInfo != null)
            {
                var localSeenBoards = new HashSet<SuperSlimBoard>();
                foreach (var bc in treeInfo)
                {
                    var childBoard = new SuperSlimBoard(board._trace, bc.Alpha, bc.Beta, bc.Response, board._stackCount);
                    if (localSeenBoards.Contains(childBoard))
                        continue;
                    localSeenBoards.Add(childBoard);

                    if (!win && seenBoards.ContainsKey(childBoard))
                        continue;

                    var childTree = BuildGameTree(childBoard, seenBoards, win, depth + 1);
                    tree.AddChild(childTree, bc);
                }
            }

            return tree;
        }
    }
}
