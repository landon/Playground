using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class OneFactorizationsOfCompleteGraph
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
                lists[i] = list ^ (1 << (i - 1));

            var priorNeighbors = new List<List<int>>();


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
                    lists[i] = lists[i].ClearBit(c);
                    lists[j] = lists[j].ClearBit(c);

                    coloring[i * n - i * (i + 1) / 2 + j - i - 1] = c;
                    foreach (var of in EnumerateOneFactorizations(i, j + 1, n, lists, coloring))
                        yield return of;

                    lists[i] = lists[i].SetBit(c);
                    lists[j] = lists[j].SetBit(c);
                }
            }
        }

        public static int GetSignSum(int n, out int plus, out int minus, Action<int, int> progress)
        {
            plus = 0;
            minus = 0;

            int list = 0;
            for (int i = 0; i < n - 1; i++)
                list |= 1 << i;

            var of = new int[n, n];
            for (int i = 1; i < n; i++)
                of[0, i] = of[i, 0] = i - 1;

            var lists = new int[n];
            for (int i = 1; i < n; i++)
                lists[i] = list ^ (1 << (i - 1));

            int term = n * (n - 1) % 4 == 0 ? 1 : -1;

            DoSignSum(1, 2, n, lists, of, ref term, ref plus, ref minus, progress);

            return plus - minus;
        }

        static void DoTerm(int term, ref int plus, ref int minus, Action<int, int> progress)
        {
            if (term > 0)
                plus++;
            else
                minus++;

            if ((plus + minus) % 1000000 == 0)
            {
                if (progress != null)
                    progress(plus, minus);
            }
        }

        static void DoSignSum(int i, int j, int n, int[] lists, int[,] of, ref int term, ref int plus, ref int minus, Action<int, int> progress)
        {
            if (i >= n - 1)
            {
                DoTerm(term, ref plus, ref minus, progress);
            }
            else
            {
                var x = lists[i] & lists[j];
                while (x != 0)
                {
                    var c = x & -x;
                    x ^= c;
                    lists[i] ^= c;
                    lists[j] ^= c;

                    of[i, j] = of[j, i] = c;

                    var factor = 1;
                    for (int k = 0; k < i; k++)
                        factor *= 1 | ((c - of[k, j]) >> 31);
                    for (int k = 0; k < j; k++)
                        factor *= 1 | ((c - of[i, k]) >> 31);

                    term *= factor;
                    if (j >= n - 1)
                        DoSignSum(i + 1, i + 2, n, lists, of, ref term, ref plus, ref minus, progress);
                    else
                        DoSignSum(i, j + 1, n, lists, of, ref term, ref plus, ref minus, progress);
                    term *= factor;

                    lists[i] ^= c;
                    lists[j] ^= c;
                }
            }
        }
    }
}
