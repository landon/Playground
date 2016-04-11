using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Inequality : Expression
    {
        public string Name { get; }
        public Expression A { get; }
        public Expression B { get; }

        public Inequality(Expression a, Expression b, string name)
        {
            A = a;
            B = b;
            Name = name;
        }

        public override void Simplify()
        {
            A.Simplify();
            B.Simplify();
        }

        public override string ToString()
        {
            return A.ToString() + " " + Name + " " + B.ToString();
        }
    }
}
