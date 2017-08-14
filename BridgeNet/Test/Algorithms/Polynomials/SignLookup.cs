using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Polynomials
{
    public class SignLookup
    {
        public int[,] Sign;
        public SignLookup(int[] height)
        {
            var m = height.Max();

            Sign = new int[m + 1, m + 1];
            for (int i = 0; i <= m; i++)
                for (int j = 0; j <= m; j++)
                    Sign[i, j] = Math.Sign((decimal)(i - j));
        }
    }
}
