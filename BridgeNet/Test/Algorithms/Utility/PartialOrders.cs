using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Utility
{
    public struct PartialOrderResult
    {
        public bool AtMost;
        public bool AtLeast;
    }

    public static class PartialOrder
    {
        public static bool Embeds<T>(List<T> A, List<T> B, Func<T, T, PartialOrderResult> order)
        {
            if (A.Count > B.Count)
                return false;

            var n = A.Count + B.Count;
            var adjacencies = new List<List<int>>(A.Count);

            foreach (var a in A)
            {
                var neighbors = new List<int>();
                for(int i = 0; i < B.Count; i++)
                {
                    var result = order(a, B[i]);
                    if (result.AtMost)
                        neighbors.Add(i);
                }

                adjacencies.Add(neighbors);
            }

            return Flows.BipartiteMaximumMatching(n, adjacencies) == A.Count;
        }
    }
}
