using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class GameTreeInfo : List<BreakerChoiceInfo>
    {
        public void Add(List<ulong> breakerChoice, int i, int j, ulong fixerResponse)
        {
            Add(new BreakerChoiceInfo()
            {
                Partition = breakerChoice.Select(choice => choice.ToSet()).ToList(),
                Alpha = i,
                Beta = j,
                Response = fixerResponse,
                SwapVertices = fixerResponse.ToSet()
            });
        }
    }

    public class BreakerChoiceInfo
    {
         public int Alpha { get; set; }
         public int Beta { get; set; }
         public List<List<int>> Partition { get; set; }
         public ulong Response { get; set; }
         public List<int> SwapVertices { get; set; }
    }
}
