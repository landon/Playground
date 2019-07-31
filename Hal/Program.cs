using System;
using System.Collections.Generic;
using System.Linq;

namespace Hal
{
    class Program
    {
        static void Main(string[] args)
        {

        }

        static void Hal(int[,] m, List<int> U, Func<List<int>, int, int>[] chooser, int k)
        {
            int p = 0;

            while (U.Count > 0)
            {
                var i = Enumerable.Range(1, k).FirstOrDefault(j => m[U[p], j] > 0);
                if (i == 0)
                {
                    U.RemoveAt(p);
                    p = 0;
                }
                else
                {
                    m[U[p], i] = m[U[p], i] - 1;
                    p = chooser[i](U, p);
                }
            }
        }

        static void Hal2(int[,] m, Func<int[,], int, int>[] chooser, int k, int n)
        {
            int Up = 0;
            var collected = new HashSet<int>();

            while (collected.Count < n)
            {
                if (collected.Contains(Up))
                    Up = Enumerable.Range(0, n).Except(collected).First();

                var i = Enumerable.Range(1, k).FirstOrDefault(j => m[Up, j] > 0);
                if (i == 0)
                {
                    collected.Add(Up);
                }
                else
                {
                    m[Up, i] = m[Up, i] - 1;
                    Up = chooser[i](m, Up);
                }
            }
        }
    }
}
