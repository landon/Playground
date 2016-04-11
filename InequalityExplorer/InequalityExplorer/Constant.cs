using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Constant : Expression
    {
        public double Value { get; }

        public Constant(double value)
        {
            Value = value;
        }

        public override void Simplify()
        {
        }

        public override string ToString()
        {
            return Value.ToString("#.##");
        }
    }
}
