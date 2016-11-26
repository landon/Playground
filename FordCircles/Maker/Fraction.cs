using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker
{
    class Fraction
    {
        public int Top;
        public int Bottom;

        public override string ToString()
        {
            return Top + "/" + Bottom;
        }
    }
}
