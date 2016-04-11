using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Sum : Expression
    {
        public List<Expression> Terms { get; }

        public Sum(Expression a, Expression b)
        {
            Terms = new List<Expression>() { a, b };
        }
        public Sum(List<Expression> terms)
        {
            Terms = terms;
        }

        public override void Simplify()
        {
            while (true)
            {
                var sums = Terms.Where(t => t is Sum).Cast<Sum>().ToList();
                if (sums.Count <= 0)
                    break;
                foreach (var sum in sums)
                    Terms.Remove(sum);
                foreach (var sum in sums)
                    Terms.AddRange(sum.Terms);
            }

            var constants = Terms.Where(t => t is Constant).Cast<Constant>().ToList();
            if (constants.Count > 1)
            {
                var pp = 0.0;
                foreach (var p in constants)
                {
                    Terms.Remove(p);
                    pp += p.Value;
                }

                Terms.Add(new Constant(pp));
            }

            foreach (var t in Terms)
                t.Simplify();
        }

        public override string ToString()
        {
            return string.Join(" + ", Terms.Select(t => t.ToString()));
        }
    }
}
