using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexTiling
{
    class Program
    {
        static void Main(string[] args)
        {
            var colors = new[] { "black", "red!65!white", "magenta!65!white", "yellow!65!white", "cyan!65!white", "green!65!white", "gray!65!white", "blue!55!white" };
            //1,4,6,2,4,7,2,5,7,3,5

            var width = 16;
            var height = 16;

            var sb = new StringBuilder();

            var top = 1;
            for (int i = 0; i < width; i++)
            {
                var scope = new Scope(i * 1.5, -(i % 2) * 0.5 * Math.Sqrt(3));
                var v = top;
                for (int j = 0; j < height; j++)
                {
                    scope.AddNode(colors[v], 0, -j * Math.Sqrt(3), "");
                    v++;
                    if (v > 7)
                        v -= 7;
                }


                top = top + 3 - (i % 2);
                if (top > 7)
                    top -= 7;

                sb.AppendLine(scope.ToString());
            }

            var tikz = sb.ToString();

            using (var sw = new StreamWriter("plane.tex"))
            {
                sw.WriteLine(@"\documentclass[crop,tikz]{standalone}");
                sw.WriteLine(@"\usepackage{tkz-graph}");
                sw.WriteLine(@"\usetikzlibrary{shapes}
\usetikzlibrary{shapes.geometric}");
                sw.WriteLine(@"\begin{document}
\begin{tikzpicture}");
                sw.WriteLine(tikz);
                sw.WriteLine(@"\end{tikzpicture}");
                sw.WriteLine(@"\end{document}");
            }
        }
    }
}
