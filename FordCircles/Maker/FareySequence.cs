using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker
{
    class FareySequence : IEnumerable<Fraction>
    {
        public int N;

        public FareySequence(int n)
        {
            N = n;
        }

        public IEnumerator<Fraction> GetEnumerator()
        {
            var a = new Fraction() { Top = 0, Bottom = 1 };
            var b = new Fraction() { Top = 1, Bottom = N };

            while (a.Top < N)
            {
                yield return a;

                var p = (N + a.Bottom) / b.Bottom * b.Top - a.Top;
                var q = (N + a.Bottom) / b.Bottom * b.Bottom - a.Bottom;

                a.Top = b.Top;
                a.Bottom = b.Bottom;
                b.Top = p;
                b.Bottom = q;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
