using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class TemplateKnowledge
    {
        Dictionary<int, PotKnowledge> _potLookup = new Dictionary<int, PotKnowledge>();

        public Template Template { get; private set; }
        public IEnumerable<Board> LostBoards { get { return _potLookup.Values.SelectMany(pk => pk.LostBoards).Distinct(); } }

        public PotKnowledge this[int colorCount]
        {
            get
            {
                PotKnowledge p;
                if (!_potLookup.TryGetValue(colorCount, out p))
                {
                    p = new PotKnowledge(colorCount);
                    _potLookup[colorCount] = p;
                }

                return p;
            }
        }

        public TemplateKnowledge(Template template)
        {
            Template = template;
        }

        public void Promote(int colorCount)
        {
            this[colorCount].CopyInto(this[colorCount + 1]);
        }
        public bool KnowledgeExists(int colorCount)
        {
            return _potLookup.ContainsKey(colorCount);
        }
    }
}
