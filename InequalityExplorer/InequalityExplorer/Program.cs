using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new Atom("a");
            var b = new Atom("b");

            var c = (new Constant(7.2) + a + b + new Constant(8)) * (a + b) >= new Constant(5);
            c.Simplify();

            Console.Write(c.ToString());
            Console.ReadKey();
        }
    }
}
