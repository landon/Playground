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

            _lazyVertexSubsets = new Lazy<List<List<int>>>(() => EnumerateVertexSubsets(0).Select(en => en.ToList()).ToList());
        }

        IEnumerable<IEnumerable<int>> EnumerateVertexSubsets(int i)
        {
            if (i >= N)
            {
                yield return new int[0];
                yield break;
            }

            foreach (var subset in EnumerateVertexSubsets(i + 1))
            {
                yield return subset;
                yield return new[] { i }.Concat(subset);
            }
        }

        public bool IsChoosable(Func<int, List<int>> getColors)
        {
            var colors = Enumerable.Range(0, N).Select(getColors).ToList();
            return IsChoosable(colors, 0, new List<int>());
        }

        public bool IsChoosableWithoutVertex(int v, Func<int, List<int>> getColors)
        {
            var colors = Enumerable.Range(0, N).Select(getColors).ToList();

            return IsChoosable(colors, v == 0 ? 1 : 0, new List<int>() { v });
        }

        bool IsChoosable(List<List<int>> lists, int i, List<int> skip)
        {
            if (i >= lists.Count)
                return true;

            var neighbors = Enumerable.Range(0, N).Where(j => _adjacent[i, j]).ToList();
            foreach (var c in lists[i])
            {
                var modified = lists.Select((ll, j) =>
                    {
                        if (neighbors.Contains(j))
                        {
                            var llc = ll.ToList();
                            llc.Remove(c);

                            return llc;
                        }
                        else
                        {
                            return ll.ToList();
                        }
                    }).ToList();

                var q = i + 1;
                while (skip.Contains(q))
                    q++;

                if (IsChoosable(modified, q, skip))
                    return true;
            }

            return false;
        }

        public int EdgeCountOn(List<int> set)
        {
            var degreeSum = 0;

            foreach (var v in set)
            {
                foreach (var w in set)
                {
                    if (_adjacent[v, w])
                        degreeSum++;
                }
            }

            return degreeSum / 2;
        }
    }
}
