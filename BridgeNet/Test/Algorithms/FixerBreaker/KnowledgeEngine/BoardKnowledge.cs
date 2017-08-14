using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class BoardKnowledge
    {
        public Board Board { get; private set; }
        public string Reason { get; private set; }

        public int Depth { get; set; }
        public Tuple<int, int> ColorPair { get; set; }
        public List<Tuple<List<List<int>>, List<Move>, string>> Swaps { get; set; }

        public BoardKnowledge(Board board, string reason)
        {
            Board = board;
            Reason = reason;
        }
    }
}
