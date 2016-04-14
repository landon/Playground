using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Product : Expression
    {
        public List<Expression> Factors { get; }

        public Product(Expression a, Expression b)
        {
            Factors = new List<Expression>() { a, b };
        }
        public Product(List<Expression> factors)
        {
            Factors = factors;
        }

        public override void Simplify()
        {
            while (true)
            {
                var products = Factors.Where(t => t is Product).Cast<Product>().ToList();
                if (products.Count <= 0)
                    break;
                foreach (var p in products)
                    Factors.Remove(p);
                foreach (var p in products)
                    Factors.AddRange(p.Factors);
            }

            var constants = Factors.Where(t => t is Constant).Cast<Constant>().ToList();
            if (constants.Count > 1)
            {
                var pp = 1.0;
                foreach (var p in constants)
                {
                    Factors.Remove(p);
                    pp *= p.Value;
                }

                Factors.Add(new Constant(pp));
            }

            foreach (var p in Factors)
                p.Simplify();
        }

        public override string ToString()
        {
            return string.Join("", Factors.Select(t => "(" + t.ToString() + ")").OrderBy(x => x));
        }
    }
}
