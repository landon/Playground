using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithms.Utility
{
    public class Factoradic
    {
        public Factoradic(long value, int n)
        {
            Value = value;

            Digits = new List<int>(n);
            Digits.Add(0);

            int i = 1;
            while (value > 0)
            {
                Digits.Add((int)(value % ++i));
                value = value / i;
            }

            for (i = Digits.Count; i < n; i++)
                Digits.Add(0);
        }

        public Factoradic(Permutation p)
        {
            Digits = new List<int>(p.N);

            List<int> elements = new List<int>(p.N);
            for (int i = 0; i < p.N; i++)
                elements.Add(i);

            for (int i = 0; i < p.N; i++)
            {
                Digits.Add(elements.IndexOf(p[i]));
                elements.Remove(p[i]);
            }

            Digits.Reverse();

            Value = 0;
            int n = 1;
            for (int i = 0; i < Digits.Count; i++)
            {
                Value += Digits[i] * n;
                n *= (i + 1);
            }
                
        }

        public List<int> Digits
        {
            get;
            private set;
        }

        public long Value
        {
            get;
            private set;
        }
    }
}