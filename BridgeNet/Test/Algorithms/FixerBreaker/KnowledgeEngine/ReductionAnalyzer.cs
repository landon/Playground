using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class ReductionAnalyzer : IBoardAnalyzer
    {
        public string Reason
        {
            get { return "reducible"; }
        }

        public bool IsKnowledgeDependent
        {
            get { return false; }
        }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            var g = knowledge.GraphKnowledge.Graph;

            foreach (var e in knowledge.GraphKnowledge.Edges)
            {
                var x = Math.Min(e.Item1, e.Item2);
                var y = Math.Max(e.Item1, e.Item2);

                var common = board.Stacks[x] & board.Stacks[y];
                var stacks = board.Stacks.ToList();
                
                foreach (var c in common.EnumerateBits())
                {
                    stacks[x] = board.Stacks[x].ClearBit(c);
                    stacks[y] = board.Stacks[y].ClearBit(c);

                    if (g.DegreeCondition(stacks, board.Pot, X => X.Contains(x) && X.Contains(y) ? 1 : 0))
                    {
                        knowledge[board.Template.Value][board.ColorCount].AddWin(board, string.Format("color {0}{1} with {2}", x + 1, y + 1, c));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
