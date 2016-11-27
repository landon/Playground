using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker
{
    class Program
    {
        const int N = 5;

        static void Main(string[] args)
        {
            using (var sw = new StreamWriter("Ford" + N + ".tex"))
            {
                sw.WriteLine(@"\documentclass{standalone}");
                sw.WriteLine(@"\usepackage{tikz}");
                sw.WriteLine(@"\begin{document}");
                sw.WriteLine(@"\begin{tikzpicture}");
                var circles = string.Join(Environment.NewLine, new FareySequence(N).Select(f => new FordCircle(f)).Select(f => string.Format(@"\draw[black, line width=0.01pt, fill={3}] ({0},{1}) circle [radius={2}];", f.X, f.Y, f.R, Fill(f.F))));
                sw.WriteLine(circles);
                sw.WriteLine(@"\end{tikzpicture}");
                sw.WriteLine(@"\end{document}");

                
            }
        }

        static string Fill(Fraction f)
        {
            var a = f.Top;
            var b = f.Bottom;

            if (a % 3 == b % 3)
                return "red";
            if (a % 3 != (b % 3) + 1)
                return "green";
            return "blue";
        }
    }
}
