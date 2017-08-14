using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class ColorPairOutcome
    {
        public Tuple<int, int> Colors;
        public IEnumerable<FixerOutcome> FixerOutcomes;
    }
}
