using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class KiersteadAnalyzer : IBoardAnalyzer
    {
        public string Reason
        {
            get { return "run kierstead"; }
        }

        public bool IsKnowledgeDependent
        {
            get { return false; }
        }

        public bool Analyze(Knowledge knowledge, Board board)
        {
            return IsKierstead(knowledge, board, true, Reason);
        }

        public static bool IsKierstead(Knowledge knowledge, Board board, bool enforceDegreeRequirement = false, string reason = "")
        {
            return EnumerateKiersteadEdgeIndices(knowledge, board, enforceDegreeRequirement, reason).Any();
        }

        public static IEnumerable<int> EnumerateKiersteadEdgeIndices(Knowledge knowledge, Board board, bool enforceDegreeRequirement = false, string reason = "")
        {
            var g = knowledge.GraphKnowledge.Graph;
            var internalVertices = g.Vertices.Where(v => g.Degree(v) >= 2).ToList();

            foreach (var edgeIndex in knowledge.GraphKnowledge.EdgeIndices)
            {
                var e = knowledge.GraphKnowledge.Edges[edgeIndex];
                if (internalVertices.Contains(e.Item1) && internalVertices.Contains(e.Item2))
                    continue;

                if (IsKierstead(knowledge, board, edgeIndex))
                {
                    knowledge[board.Template.Value][board.ColorCount].AddWin(board, reason);
                    yield return edgeIndex;
                }
            }
        }

        public static bool IsKierstead(Knowledge knowledge, Board board, int edgeIndex, bool enforceDegreeRequirement = false)
        {
            foreach (var ordering in knowledge.GraphKnowledge.ConnectedOrderings.Value[knowledge.GraphKnowledge.Edges[edgeIndex]])
            {
                var penultimate = ordering[ordering.Count - 2];
                if (enforceDegreeRequirement && board.Stacks[penultimate].PopulationCount() <= knowledge.GraphKnowledge.Graph.Degree(penultimate))
                    continue;

                if (IsKierstead(knowledge, board, edgeIndex, ordering))
                    return true;
            }

            return false;
        }

        public static bool IsKierstead(Knowledge knowledge, Board board, int edgeIndex, List<int> ordering)
        {
            var gn = knowledge.GraphKnowledge;

            return gn.LineGraph.IsChoosable(Enumerable.Range(0, gn.LineGraph.N).Select(e =>
            {
                if (e == edgeIndex)
                    return -1;

                var edge = gn.Edges[e];
                var j = Math.Max(ordering.IndexOf(edge.Item1), ordering.IndexOf(edge.Item2));
                var priorStacks = board.Stacks.Where((stack, i) => ordering.IndexOf(i) < j).ToList();

                long allowedList = 0;
                var list = knowledge.GetEdgeColorList(board, e);
                foreach (var alpha in list.EnumerateBits())
                {
                    if (MetaKnowledge.d_H(alpha, priorStacks) % 2 == 0)
                        allowedList = allowedList.SetBit(alpha);
                }

                return allowedList;
            }).ToList());
        }
    }
}
