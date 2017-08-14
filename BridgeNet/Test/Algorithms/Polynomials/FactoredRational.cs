using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Polynomials
{
    public class FactoredRational
    {
        public readonly static FactoredRational Zero = new FactoredRational() { Sign = 0 };

        public int Sign;
        public List<int> K;

        public FactoredRational() { }

        public FactoredRational(int n)
        {
            var vv = new int[11];
            for (int jj = 0; jj <= 10; jj++)
                vv[jj] = Power(2, jj);

            Sign = Math.Sign((decimal)n);
            n *= Sign;

            K = new List<int>();
            int i = 0;

            while (n > 1)
            {
                K.Add(0);
                var p = PrimeNumbers.Get(i);
                while (n % p == 0)
                {
                    K[i]++;
                    n /= p;
                }

                i++;
            }
        }

        public static FactoredRational operator *(FactoredRational fi, FactoredRational fi2)
        {
            var sign = fi.Sign * fi2.Sign;
            var k = new List<int>(Math.Max(fi.K.Count, fi2.K.Count));

            if (sign != 0)
            {
                for (int i = 0; i < Math.Max(fi.K.Count, fi2.K.Count); i++)
                {
                    k.Add(0);
                    if (i < fi.K.Count)
                        k[i] += fi.K[i];
                    if (i < fi2.K.Count)
                        k[i] += fi2.K[i];
                }
            }

            return new FactoredRational() { K = k, Sign = sign };
        }

        public static FactoredRational operator /(FactoredRational fi, FactoredRational fi2)
        {
            var sign = fi.Sign * fi2.Sign;
            var k = new List<int>(Math.Max(fi.K.Count, fi2.K.Count));

            if (sign != 0)
            {
                for (int i = 0; i < Math.Max(fi.K.Count, fi2.K.Count); i++)
                {
                    k.Add(0);
                    if (i < fi.K.Count)
                        k[i] += fi.K[i];
                    if (i < fi2.K.Count)
                        k[i] -= fi2.K[i];
                }
            }

            return new FactoredRational() { K = k, Sign = sign };
        }

        public void ToRational(out int top, out int bottom)
        {
            if (K == null || Sign == 0)
            {
                top = 0;
                bottom = 1;
                return;
            }

            top = Sign;
            bottom = 1;
            for (int i = 0; i < K.Count; i++)
            {
                if (K[i] > 0)
                    top *= Power(PrimeNumbers.Get(i), K[i]);
                else
                    bottom *= Power(PrimeNumbers.Get(i), -K[i]);
            }
        }

        int Power(int p, int k)
        {
            var m = 1;
            while (k > 0)
            {
                var e = 1;
                var n = p;
                while (e <= k >> 1)
                {
                    n *= n;
                    e <<= 1;
                }

                k -= e;
                m *= n;
            }

            return m;
        }
    }
}
