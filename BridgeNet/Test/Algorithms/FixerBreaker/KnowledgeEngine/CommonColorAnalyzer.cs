using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class CommonColorAnalyzer : IBoardAnalyzer
    {
        public string Reason
        {
            get { return "common color"; }
        }

        public bool IsKnowledgeDependent
        {
            get { return false; }
        }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            var commonColor = board.Stacks.Aggregate(-1L, (t, s) => t & s) != 0;

            if (commonColor)
                knowledge[board.Template.Value][board.ColorCount].AddWin(board, Reason);

            return commonColor;
        }
    }
}
