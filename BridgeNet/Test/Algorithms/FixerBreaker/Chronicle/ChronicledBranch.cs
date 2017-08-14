using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.Chronicle
{
    public class ChronicledBranch : IEnumerable<IEnumerable<Move>>
    {
        public int Alpha {get; private set;}
        public int Beta {get; private set;}
        public List<List<int>> SwapComponents { get; private set; }

        public ChronicledBranch(List<int> S, int alpha, int beta, List<List<int>> partitions)
        {
            Alpha = alpha;
            Beta = beta;
            SwapComponents = partitions.Select(p => p.Select(i => S[i]).ToList()).ToList();
        }

        public IEnumerator<IEnumerable<Move>> GetEnumerator()
        {
            return ListUtility.EnumerateShortLexNonempty(SwapComponents.Count).Select(cf => EnumerateMoves(cf)).GetEnumerator();
        }

        IEnumerable<Move> EnumerateMoves(List<bool> cf)
        {
            for (int i = 0; i < cf.Count; i++)
            {
                if (!cf[i])
                    continue;

                foreach (var s in SwapComponents[i])
                    yield return new Move() { Added = Alpha, Removed = Beta, Stack = s };
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
