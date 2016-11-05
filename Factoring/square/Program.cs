using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace square
{
    class Program
    {
        static Random RNG = new Random(DateTime.Now.Millisecond);

        static void Main(string[] args)
        {
            var v = Factor(13*19*23);
        }

        static int Factor(int n)
        {
            while (true)
            {
                var a = RNG.Next(2, n - 1);
                if (GCD(a, n) != 1)
                    continue;

                var p = FindPeriod(a, n);
                if (p % 2 == 1)
                    continue;

                var A = new BigInteger(a);
                var z = BigInteger.Pow(A, p / 2);
                var b = z % n;
                if (b == 1 || b == n - 1)
                    continue;
                var bb = (int)b;
                return GCD(bb - 1, n);
            }
        }

        static int FindPeriod(int a, int n)
        {
            var A = new BigInteger(a);
            int i = 1;
            while (true)
            {
                var x = BigInteger.Pow(A, i);
                var y = x % n;
                if (y == 1)
                    return i;
                i++;
            }
        }

        static int GCD(int a, int b)
        {
            if (a == 0)
                return b;
            if (b == 0)
                return a;
            if (a == b)
                return a;
            if (a < b)
                return GCD(a, b % a);
            return GCD(a % b, b);
        }
    }
}
