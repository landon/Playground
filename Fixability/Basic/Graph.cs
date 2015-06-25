using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class Graph : IGraph<List<int>, List<int>>
    {
        bool[,] _adjacent;
        Lazy<List<List<int>>> _lazyVertexSubsets;

        public int N { get; private set; }
        public List<List<int>> VertexSubsets { get { return _lazyVertexSubsets.Value; } }
        public IOperations<List<int>, List<int>> Operations { get { return Basic.Operations.Instance; } }

        public bool AreAdjacent(int v, int w)
        {
            return _adjacent[v, w];
        }

        public void Initialize(bool[,] adjacent)
        {
            _adjacent = adjacent;
            N = _adjacent.GetUpperBound(0) + 1;
        }

        public bool IsChoosable(Func<int, List<int>> geTColorSet)
        {
            throw new NotImplementedException();
        }

        public bool IsChoosableWithoutVertex(int v, Func<int, List<int>> geTColorSet)
        {
            throw new NotImplementedException();
        }

        public int EdgeCountOn(List<int> set)
        {
            throw new NotImplementedException();
        }
    }
}
