using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class ColoringAnalyzer : IBoardAnalyzer
    {
        public string Reason { get { return "color all"; } }
        public bool IsKnowledgeDependent { get { return false; } }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            var lineGraph = knowledge.GraphKnowledge.LineGraph;
            var canColor = lineGraph.IsChoosable(Enumerable.Range(0, lineGraph.N).Select(e => knowledge.GetEdgeColorList(board, e)).ToList());

            if (canColor)
                knowledge[board.Template.Value][board.ColorCount].AddWin(board, Reason);

            return canColor;
        }

        public static bool ColorableWithoutEdge(Knowledge knowledge, Board board, int edgeIndex)
        {
            return ColorableWithoutEdge(knowledge, board.Stacks, edgeIndex);
        }

        public static bool ColorableWithoutEdge(Knowledge knowledge, List<long> stacks, int edgeIndex)
        {
            var lg = knowledge.GraphKnowledge.LineGraph;

            return lg.IsChoosable(Enumerable.Range(0, lg.N).Select(e =>
            {
                if (e == edgeIndex)
                    return -1;

                return knowledge.GetEdgeColorList(stacks, e);
            }).ToList());
        }
    }
}
