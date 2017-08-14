using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Polynomials
{
    public class GraphPolynomial
    {
        Algorithms.Graph _g;
        List<List<int>> _priorNeighbors;
        Normalizer _normalizer;
        SignLookup _signLookup;
        
        public GraphPolynomial(Algorithms.Graph g)
        {
            _g = g;
       
            _priorNeighbors = new List<List<int>>();
            for (int w = 0; w < _g.N; w++)
            {
                _priorNeighbors.Add(new List<int>());
                for (int v = 0; v < w; v++)
                {
                    if (g[v, w])
                        _priorNeighbors[w].Add(v);
                }
            }
        }

        public int GetCoefficient(int[] power)
        {
            _normalizer = new Normalizer(power);

            var a = new int[power.Length];
            long top = 0L;
            long bottom = 1L;
            var partialProduct = new FactoredRational() { K = new List<int>(), Sign = 1 };
            SumTerms(0, a, power, ref partialProduct, ref top, ref bottom);

            return (int)(top / bottom);
        }

        public int GetSignSum(int[] power)
        {
            _signLookup = new SignLookup(power);

            var a = new int[power.Length];
            var sum = 0;
            var partialProduct = 1;
            SumSigns(0, a, power, ref partialProduct, ref sum);

            return sum;
        }

        void SumSigns(int w, int[] a, int[] power, ref int partialProduct, ref int sum)
        {
            if (w == power.Length)
            {
                sum += partialProduct;
            }
            else
            {
                for (int j = 0; j <= power[w]; j++)
                {
                    a[w] = j;

                    var current = 1;
                    foreach (var v in _priorNeighbors[w])
                        current *= _signLookup.Sign[a[v], a[w]];

                    if (current == 0)
                        continue;

                    partialProduct *= current;
                    SumSigns(w + 1, a, power, ref partialProduct, ref sum);
                    partialProduct /= current;
                }
            }
        }

        void SumTerms(int w, int[] a, int[] power, ref FactoredRational partialProduct, ref long top, ref long bottom)
        {
            if (w == power.Length)
            {
                var r = partialProduct / _normalizer.Get(a);
                int top2;
                int bottom2;
                r.ToRational(out top2, out bottom2);

                top = top * bottom2 + bottom * top2;
                bottom = bottom * bottom2;

                var gcd = GCD(top, bottom);
                top /= gcd;
                bottom /= gcd;
            }
            else
            {
                for (int j = 0; j <= power[w]; j++)
                {
                    a[w] = j;

                    var current = new FactoredRational() { K = new List<int>(), Sign = 1 };
                    foreach (var v in _priorNeighbors[w])
                        current *= _normalizer.FactoredDifference[a[v], a[w]];

                    if (current.Sign == 0)
                        continue;

                    partialProduct *= current;
                    SumTerms(w + 1, a, power, ref partialProduct, ref top, ref bottom);
                    partialProduct /= current;
                }
            }
        }

        long GCD(long x, long y)
        {
            long tmp;
            x = Math.Abs(x);
            y = Math.Abs(y);
            while (x > 0)
            {
                tmp = x;
                x = y % x;
                y = tmp;
            }
            return y;
        }
    }

    public static class GraphPolynomialExtensions
    {
        public static int GetCoefficient(this Algorithms.Graph g, int[] power)
        {
            var gp = new GraphPolynomial(g);
            return gp.GetCoefficient(power);
        }

        public static int GetSignSum(this Algorithms.Graph g, int[] power)
        {
            var gp = new GraphPolynomial(g);
            return gp.GetSignSum(power);
        }
    }
}
