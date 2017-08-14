using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class GreedyWinFilter : IWinFilter
    {
        SuperSlimSwapAnalyzer _swapAnalyzer;

        public GreedyWinFilter(SuperSlimSwapAnalyzer swapAnalyzer)
        {
            _swapAnalyzer = swapAnalyzer;
        }

        public override IEnumerable<int> Filter(List<SuperSlimBoard> R, HashSet<SuperSlimBoard> W)
        {
            return R.IndicesWhere(b => _swapAnalyzer.Analyze(b, W));
        }
    }
}
