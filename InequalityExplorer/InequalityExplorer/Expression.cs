using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    abstract class Expression
    {
        public abstract void Simplify();

        public static Sum operator +(Expression a, Expression b)
        {
            return new Sum(a, b);
        }
        public static Product operator *(Expression a, Expression b)
        {
            return new Product(a, b);
        }
        public static Expression operator -(Expression a)
        {
            return new Negation(a);
        }
        public static Inequality operator >=(Expression a, Expression b)
        {
            if (a is Inequality || b is Inequality) throw new Exception("no nonsense");
            return new Inequality(a, b, "\\ge");
        }
        public static Inequality operator <=(Expression a, Expression b)
        {
            if (a is Inequality || b is Inequality) throw new Exception("no nonsense");
            return new Inequality(a, b, "\\le");
        }
        public static Inequality operator >(Expression a, Expression b)
        {
            if (a is Inequality || b is Inequality) throw new Exception("no nonsense");
            return new Inequality(a, b, ">");
        }
        public static Inequality operator <(Expression a, Expression b)
        {
            if (a is Inequality || b is Inequality) throw new Exception("no nonsense");
            return new Inequality(a, b, "<");
        }
    }
}
