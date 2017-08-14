using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.IndependenceRatio
{
    public static class MinorFinder
    {
        // TODO: replace naive implementation of this with fast version if necessary.
        // also, |J| is hardcoded to 3, which is all we need now.
        public static IEnumerable<List<Tuple<List<int>, List<int>>>> FindWithJ(Graph g, List<int> J)
        {
            if (J.Count != 3)
                yield break;

            foreach (var triple in EnumerateConnectedThreePartPartitions(g, J))
            {
                if (g.IndependenceNumber(triple.Item1.Union(J)) < 4)
                    continue;
                if (g.IndependenceNumber(triple.Item2.Union(J)) < 4)
                    continue;
                if (g.IndependenceNumber(triple.Item3.Union(J)) < 4)
                    continue;
                if (g.EdgesBetween(triple.Item1, triple.Item2) <= 0 && g.IndependenceNumber(triple.Item1.Union(triple.Item2).Union(J)) < 5)
                    continue;
                if (g.EdgesBetween(triple.Item1, triple.Item3) <= 0 && g.IndependenceNumber(triple.Item1.Union(triple.Item3).Union(J)) < 5)
                    continue;
                if (g.EdgesBetween(triple.Item2, triple.Item3) <= 0 && g.IndependenceNumber(triple.Item2.Union(triple.Item2).Union(J)) < 5)
                    continue;

                yield return new List<Tuple<List<int>, List<int>>>() { new Tuple<List<int>, List<int>>(triple.Item1.ToList(), g.MaximumIndependentSubset(triple.Item1.Union(J))),
                                                                       new Tuple<List<int>, List<int>>(triple.Item2.ToList(), g.MaximumIndependentSubset(triple.Item2.Union(J))),
                                                                       new Tuple<List<int>, List<int>>(triple.Item3.ToList(), g.MaximumIndependentSubset(triple.Item3.Union(J)))};
            }
        }

        static IEnumerable<Tuple<List<int>, List<int>, List<int>>> EnumerateConnectedThreePartPartitions(Graph g, List<int> J)
        {
            var rest = g.Vertices.Except(J).ToList();
            rest.Shuffle();
            foreach (var first in rest.EnumerateSublists())
            {
                first.Add(J[0]);
                if (g.FindComponents(first).GetEquivalenceClasses().Count() != 1)
                    continue;

                var rest2 = rest.Except(first).ToList();
                foreach (var second in rest2.EnumerateSublists())
                {
                    second.Add(J[1]);
                    if (g.FindComponents(second).GetEquivalenceClasses().Count() != 1)
                        continue;

                    var third = rest2.Except(second).ToList();
                    third.Add(J[2]);
                    if (g.FindComponents(third).GetEquivalenceClasses().Count() != 1)
                        continue;

                    yield return new Tuple<List<int>, List<int>, List<int>>(first, second, third);
                }
            }
        }
    }
}
