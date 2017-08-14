using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Polynomials
{
    public class Normalizer
    {
        public FactoredRational[,] FactoredDifference;
        public FactoredRational[,] InsideProduct;

        public Normalizer(int[] height)
        {
            var m = height.Max();

            FactoredDifference = new FactoredRational[m + 1, m + 1];
            for (int i = 0; i <= m; i++)
                for (int j = 0; j <= m; j++)
                    FactoredDifference[i, j] = new FactoredRational(i - j);

            InsideProduct = new FactoredRational[height.Length, m + 1];
            for (int i = 0; i < height.Length; i++)
            {
                for (int j = 0; j <= height[i]; j++)
                {
                    InsideProduct[i, j] = new FactoredRational() { K = new List<int>(), Sign = 1 };
                    for (int k = 0; k <= height[i]; k++)
                    {
                        if (k == j)
                            continue;

                        InsideProduct[i, j] *= FactoredDifference[j, k];
                    }
                }
            }
        }

        public FactoredRational Get(int[] a)
        {
            var normalizer = new FactoredRational() { K = new List<int>(), Sign = 1 };
            for(int i = 0; i < a.Length; i++)
                normalizer *= InsideProduct[i, a[i]];

            return normalizer;
        }
    }
}
