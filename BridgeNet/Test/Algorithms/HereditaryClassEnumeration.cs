using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class HereditaryClassEnumeration
    {
        static readonly SizeComparer _sizeComparer = new SizeComparer();

        public static IEnumerable<Graph> EnumerateClass(params Graph[] excluded)
        {
            return EnumerateClass(excluded.ToList());
        }

        public static IEnumerable<Graph> EnumerateClass(List<Graph> excluded, int maxVertices = int.MaxValue)
        {
            excluded.Sort((g1, g2) => g1.N.CompareTo(g2.N));
            
            if (excluded.Count > 0 && excluded[0].N <= 1) yield break;
            yield return new Graph(new List<int>());

            var lastLevel = new List<Graph>() { new Graph(new List<int>()) };

            while (lastLevel.Count > 0)
            {
                var currentLevel = new List<Graph>();

                foreach (var G in lastLevel)
                {
                    List<List<int>> neighborSets;

                    if (G.IsComplete())
                    {
                        neighborSets = ListUtility.EnumerateSublists(Enumerable.Range(0, G.N).ToList()).Distinct(_sizeComparer).ToList();
                    }
                    else
                    {
                        neighborSets = ListUtility.GenerateSublists(Enumerable.Range(0, G.N).ToList());
                    }
                    
                    foreach (var neighborSet in neighborSets.Where(l => l.Count > 0))
                    {
                        var H = G.AttachNewVertex(neighborSet);

                        if (currentLevel.Any(W => Graph.Isomorphic(H, W))) continue;
                        if (excluded.Any(W => H.ContainsInduced(W))) continue;

                        currentLevel.Add(H);
                        yield return H;
                    }
                }

                if (currentLevel.Count > 0 && currentLevel[0].N >= maxVertices) yield break;
                lastLevel = currentLevel;
            }
        }

        class SizeComparer : IEqualityComparer<List<int>>
        {
            public bool Equals(List<int> x, List<int> y)
            {
                return x.Count == y.Count;
            }

            public int GetHashCode(List<int> obj)
            {
                return obj.Count.GetHashCode();
            }
        }
    }
}
