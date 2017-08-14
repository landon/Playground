using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class SwapAnalyzer : IBoardAnalyzer
    {
        bool FindCleanestWin { get; set; }
        bool DoSuperabundantCheck { get; set; }
        
        public virtual string Reason { get { return "good swap"; } }
        public bool IsKnowledgeDependent { get { return true; } }
        protected virtual string ChildReason { get { return string.Empty; } }
        protected virtual bool IsChildBoardBetter(Knowledge knowledge, Board board, Board childBoard) { return false; }

        public SwapAnalyzer(bool findCleanestWin = true, bool superabundantOnly = true)
        {
            FindCleanestWin = findCleanestWin;
            DoSuperabundantCheck = superabundantOnly;
        }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            var potKnowledge = knowledge[board.Template.Value][board.ColorCount];
            var graphKnowledge = knowledge.GraphKnowledge;

            var boardKnowledge = potKnowledge[board];
            if (boardKnowledge.Exists())
                return true;

            var win = false;
            var winDepth = MetaKnowledge.Infinity;
            foreach (var colorPair in potKnowledge.ColorPairs.OrderByDescending(pair => Chronicle.BranchGenerator.EnumerateExactlyOneIntersecters(board, pair).Count() % 2))
            {
                var colorPairWinDepth = 0;
                var winningSwaps = new List<Tuple<List<List<int>>, List<Move>, string>>();
                var improvingSwaps = new List<Tuple<List<List<int>>, List<Move>, string>>();
                var winningSwapAlwaysExists = true;
                var improvingSwapAlwaysExists = true;

                foreach (var branch in Chronicle.BranchGenerator.EnumerateBranches(board, colorPair))
                {
                    Tuple<List<List<int>>, List<Move>, string> winningSwap = null;
                    Tuple<List<List<int>>, List<Move>, string> improvingSwap = null;

                    var branchWinDepth = MetaKnowledge.Infinity;

                    foreach (var swap in branch)
                    {
                        var childBoard = board.Clone();
                        childBoard.DoMoveCombination(swap);

                        if (DoSuperabundantCheck && !graphKnowledge.Graph.DegreeCondition(childBoard))
                            continue;

                        var childBoardKnowledge = potKnowledge[childBoard];
                        if (childBoardKnowledge.Exists() && childBoardKnowledge.Depth + 1 < branchWinDepth)
                        {
                            branchWinDepth = childBoardKnowledge.Depth + 1;
                            winningSwap = new Tuple<List<List<int>>, List<Move>, string>(branch.SwapComponents, swap.ToList(), childBoardKnowledge.Reason);

                            if (!FindCleanestWin)
                                break;
                        }
                        else if (IsChildBoardBetter(knowledge, board, childBoard))
                        {
                            improvingSwap = new Tuple<List<List<int>>, List<Move>, string>(branch.SwapComponents, swap.ToList(), ChildReason + " better");
                        }
                    }

                    colorPairWinDepth = Math.Max(colorPairWinDepth, branchWinDepth);

                    if (winningSwap.Exists())
                    {
                        winningSwaps.Add(winningSwap);
                        improvingSwaps.Add(winningSwap);
                    }
                    else
                    {
                        winningSwapAlwaysExists = false;
                        if (improvingSwap.Exists())
                            improvingSwaps.Add(improvingSwap);
                        else
                        {
                            improvingSwapAlwaysExists = false;
                            break;
                        }
                    }
                }

                if (winningSwapAlwaysExists)
                {
                    if (colorPairWinDepth < winDepth)
                    {
                        winDepth = colorPairWinDepth;

                        win = true;
                        potKnowledge.AddWin(board, Reason, winDepth, colorPair, winningSwaps);

                        if (!FindCleanestWin)
                            break;
                    }
                }
                else if (improvingSwapAlwaysExists)
                {
                    potKnowledge.AddImprovement(board, ChildReason, colorPairWinDepth, colorPair, improvingSwaps);
                }
            }

            return win;
        }
    }
}
