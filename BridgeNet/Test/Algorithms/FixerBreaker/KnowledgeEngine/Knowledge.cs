using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class Knowledge
    {
        Dictionary<Template, TemplateKnowledge> _templateLookup = new Dictionary<Template, TemplateKnowledge>();

        public GraphKnowledge GraphKnowledge { get; private set; }
        public TemplateKnowledge this[Template template]
        {
            get
            {
                TemplateKnowledge p;
                if (!_templateLookup.TryGetValue(template, out p))
                {
                    p = new TemplateKnowledge(template);
                    _templateLookup[template] = p;
                }

                return p;
            }
        }
        
        public Knowledge(Graph g)
        {
            GraphKnowledge = new GraphKnowledge(g);
        }

        public long GetEdgeColorList(Board board, int edgeIndex)
        {
            return board[GraphKnowledge.Edges[edgeIndex].Item1] & board[GraphKnowledge.Edges[edgeIndex].Item2];
        }
        public long GetEdgeColorList(List<long> stacks, int edgeIndex)
        {
            return stacks[GraphKnowledge.Edges[edgeIndex].Item1] & stacks[GraphKnowledge.Edges[edgeIndex].Item2];
        }
    }
}
