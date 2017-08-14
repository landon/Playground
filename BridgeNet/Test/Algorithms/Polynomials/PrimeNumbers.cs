using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Polynomials
{
    public static class PrimeNumbers
    {
        static List<int> _primes = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19 };

        public static int Get(int i)
        {
            if (i >= _primes.Count)
                ComputeMorePrimes(i);

            return _primes[i];
        }

        static void ComputeMorePrimes(int i)
        {
            var m = Math.Max(i, 2 * _primes.Count);
            var p = _primes[_primes.Count - 1] + 2;

            while (_primes.Count <= m)
            {
                if (IsPrime(p))
                    _primes.Add(p);

                p += 2;
            }
        }

        static bool IsPrime(int p)
        {
            for(int k = 0; _primes[k] * _primes[k] <= p; k++)
            {
                if (p % _primes[k] == 0)
                    return false;
            }

            return true;
        }
    }
}
