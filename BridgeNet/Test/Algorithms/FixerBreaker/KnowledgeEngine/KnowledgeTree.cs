using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class KnowledgeTree : Tree<KnowledgeTree>
    {
        public Tuple<List<List<int>>, List<Move>, string> Swap { get; private set; }
        public Tuple<int, int> ColorPair { get; set; }
        public string Board { get; set; }
        public string Note { get; set; }
        public int Number { get; set; }

        public KnowledgeTree AddChild(Tuple<List<List<int>>, List<Move>, string> swap)
        {
            var child = new KnowledgeTree() { Swap = swap };
            AddChild(child);

            return child;
        }
    }
}
