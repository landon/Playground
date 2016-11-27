using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker
{
    class FordCircle
    {
        public Fraction F;
        public double X;
        public double Y;
        public double R;

        public FordCircle(Fraction f)
        {
            F = f;
            X = (double)f.Top / f.Bottom;
            Y = 1.0 / (2.0 * f.Bottom * f.Bottom);
            R = 1.0 / (2.0 * f.Bottom * f.Bottom);
        }
    }
}