using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class Counting
    {
        public static long BinomialCoefficient(long n, long k)
        {
            if (k < 0)
            {
                return 1;
            }

            long max = Math.Max(k, n - k);
            long min = Math.Min(k, n - k);

            long result = 1;
            while (n > max)
            {
                result *= n;
                n--;
            }

            while (min > 0)
            {
                result /= min;

                min--;
            }

            return result;
        }

        public static long Factorial(int n)
        {
            long total = 1;
            for (int i = 1; i <= n; i++)
                total *= i;

            return total;
        }
    }
}
