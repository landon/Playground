using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim
{
    public class SlimSwapAnalyzer
    {
        Knowledge _knowledge;
        Dictionary<int, Board> _boardLookup;
        Dictionary<Board, int> _boardIDLookup;
        Dictionary<int, List<List<SortedIntList>>> _shrubs = new Dictionary<int, List<List<SortedIntList>>>();
        bool _useShrubs;

        public SlimSwapAnalyzer(Knowledge knowledge, Dictionary<int, Board> boardLookup, Dictionary<Board, int> boardIDLookup, bool useShrubs = false)
        {
            _knowledge = knowledge;
            _boardLookup = boardLookup;
            _boardIDLookup = boardIDLookup;
            _useShrubs = useShrubs;
        }

        public bool Analyze(int boardID, SortedIntList wonBoardIDs)
        {
            if (!_useShrubs)
                return AnalyzeShrubFree(boardID, wonBoardIDs);

            List<List<SortedIntList>> shrub;
            if (!_shrubs.TryGetValue(boardID, out shrub))
                return InitializeShrubs(boardID, wonBoardIDs);

            foreach (var colorPairList in shrub)
            {
                colorPairList.RemoveAll(branchList => branchList.Intersects(wonBoardIDs), (x) => { });
                if (colorPairList.Count <= 0)
                    return true;
            }

            return false;
        }

        bool AnalyzeShrubFree(int boardID, SortedIntList wonBoardIDs)
        {
            var board = _boardLookup[boardID];
            var potKnowledge = _knowledge[board.Template.Value][board.ColorCount];

            foreach (var colorPair in potKnowledge.ColorPairs.OrderByDescending(pair => Chronicle.BranchGenerator.EnumerateExactlyOneIntersecters(board, pair).Count() % 2))
            {
                var winningSwapAlwaysExists = true;
                foreach (var branch in Chronicle.BranchGenerator.EnumerateBranches(board, colorPair))
                {
                    var winningSwapExists = false;

                    foreach (var swap in branch)
                    {
                        board.DoMoveCombination(swap);
                        var childID = _boardIDLookup[board];
                        board.DoMoveCombination(swap);

                        if (wonBoardIDs.Contains(childID))
                        {
                            winningSwapExists = true;
                            break;
                        }
                    }

                    if (!winningSwapExists)
                    {
                        winningSwapAlwaysExists = false;
                        break;
                    }
                }

                if (winningSwapAlwaysExists)
                    return true;
            }

            return false;
        }

        bool InitializeShrubs(int boardID, SortedIntList wonBoardIDs)
        {
            var shrub = new List<List<SortedIntList>>();
            _shrubs[boardID] = shrub;

            var board = _boardLookup[boardID];
            var potKnowledge = _knowledge[board.Template.Value][board.ColorCount];

            foreach (var colorPair in potKnowledge.ColorPairs.OrderByDescending(pair => Chronicle.BranchGenerator.EnumerateExactlyOneIntersecters(board, pair).Count() % 2))
            {
                var colorPairList = new List<SortedIntList>();
                shrub.Add(colorPairList);

                var winningSwapAlwaysExists = true;
                foreach (var branch in Chronicle.BranchGenerator.EnumerateBranches(board, colorPair))
                {
                    var branchList = new SortedIntList();
                    var winningSwapExists = false;

                    foreach (var swap in branch)
                    {
                        board.DoMoveCombination(swap);
                        var childID = _boardIDLookup[board];
                        board.DoMoveCombination(swap);

                        if (wonBoardIDs.Contains(childID))
                        {
                            winningSwapExists = true;
                            break;
                        }

                        branchList.Add(childID);
                    }

                    if (!winningSwapExists)
                    {
                        colorPairList.Add(branchList);
                        winningSwapAlwaysExists = false;
                    }
                }

                if (winningSwapAlwaysExists)
                    return true;
            }

            return false;
        }
    }
}
