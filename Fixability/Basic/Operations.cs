using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class Operations : IOperations<List<int>, List<int>>
    {
        public static Operations Instance = new Operations();

        Operations() { }

        public int Count(List<int> set)
        {
            return set.Count;
        }

        public List<int> Subset(List<int> set, List<int> indices)
        {
            return indices.Select(i => set[i]).ToList();
        }
    }
}
