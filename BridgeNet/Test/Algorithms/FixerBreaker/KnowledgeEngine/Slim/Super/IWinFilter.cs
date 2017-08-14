using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public abstract class IWinFilter
    {
        public abstract IEnumerable<int> Filter(List<SuperSlimBoard> R, HashSet<SuperSlimBoard> W);
    }
}
