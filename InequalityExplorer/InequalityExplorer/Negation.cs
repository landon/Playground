using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Negation : Expression
    {
        public Expression Expression { get; private set; }

        public Negation(Expression expression)
        {
            Expression = expression;
        }

        public override void Simplify()
        {
            var x = Expression as Negation;
            if (x != null)
                Expression = x.Expression;

            Expression.Simplify();
        }

        public override string ToString()
        {
            return "-" + "(" + Expression.ToString() + ")";
        }
    }
}
