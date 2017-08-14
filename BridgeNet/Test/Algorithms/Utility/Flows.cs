using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Utility
{
    public static class Flows
    {
        public static int BipartiteMaximumMatching(int n, List<List<int>> adjacencies)
        {
            var capacity = new int[n + 2, n + 2];
            var source = 0;
            var target = 1;

            var lo = 2;
            var ro = lo + adjacencies.Count;

            for (int i = 0; i < adjacencies.Count; i++)
            {
                capacity[source, lo + i] = 1;

                foreach (var j in adjacencies[i])
                {
                    capacity[lo + i, ro + j] = 1;
                    capacity[ro + j, target] = 1;
                }
            }

            return MaxFlow(capacity, source, target);
        }

        public static int MaxFlow(int[,] capacity, int source, int target)
        {
            return new FordFulkerson(capacity, source, target).MaxFlow();
        }

        class FordFulkerson
        {
            int _n;
            int _source;
            int _target;
            int[,] _capacity;
            int[,] _flow;

            public FordFulkerson(int[,] capacity, int source, int target)
            {
                _capacity = capacity;
                _source = source;
                _target = target;

                _n = capacity.GetLength(0);
                _flow = new int[_n, _n];
            }

            public int MaxFlow()
            {
                while (true)
                {
                    var path = new List<int>() { 0 };
                    var done = !FindAugmentingPath(path);
                    if (done)
                        break;

                    AddFlow(path);
                }

                return Enumerable.Range(0, _n).Sum(i => _flow[_source, i]);
            }

            bool FindAugmentingPath(List<int> path)
            {
                var last = path[path.Count - 1];

                if (last == _target)
                    return true;

                foreach (var j in OutNeighbors(last))
                {
                    if (path.Contains(j))
                        continue;

                    path.Add(j);

                    var isAugmenting = FindAugmentingPath(path);
                    if (isAugmenting)
                        return true;

                    path.Remove(j);
                }

                return false;
            }

            IEnumerable<int> OutNeighbors(int i)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (_capacity[i, j] - _flow[i, j] > 0)
                        yield return j;
                }
            }

            void AddFlow(List<int> path)
            {
                var gain = FindGain(path);

                for (int i = 0; i < path.Count - 1; i++)
                {
                    _flow[path[i], path[i + 1]] += gain;
                    _flow[path[i + 1], path[i]] = -_flow[path[i], path[i + 1]];
                }
            }

            int FindGain(List<int> path)
            {
                var gain = int.MaxValue;

                for (int i = 0; i < path.Count - 1; i++)
                    gain = Math.Min(gain, _capacity[path[i], path[i + 1]] - _flow[path[i], path[i + 1]]);

                return gain;
            }
        }
    }
}
