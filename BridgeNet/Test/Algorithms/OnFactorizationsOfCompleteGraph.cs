using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class OnFactorizationsOfCompleteGraph
    {
        public static IEnumerable<int[]> EnumerateOneFactorizations(int n)
        {
            long list = 0;
            for (int i = 0; i < n - 1; i++)
                list |= 1L << i;

            var coloring = new int[n * (n - 1) / 2];
            for (int i = 0; i < n - 1; i++)
                coloring[i] = i;

            var lists = new long[n];
            for (int i = 1; i < n; i++)
            {
                lists[i] = list ^ (1 << (i - 1));

                coloring[i * n - i * (i + 1) / 2] = i - 1;
            }

            return EnumerateOneFactorizations(1, 2, n, lists, coloring);
        }

        static IEnumerable<int[]> EnumerateOneFactorizations(int i, int j, int n, long[] lists, int[] coloring)
        {
            if (i >= n)
            {
                var copy = new int[coloring.Length];
                Array.Copy(coloring, copy, coloring.Length);
                yield return copy;
            }
            else if (j >= n)
            {
                foreach (var of in EnumerateOneFactorizations(i + 1, i + 2, n, lists, coloring))
                    yield return of;
            }
            else
            {
                foreach (var c in (lists[i] & lists[j]).EnumerateBits())
                {

                }
            }
        }
    }
}
