using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexTiling
{
    class Program
    {
        static void Main(string[] args)
        {
            var colors = new[] { "black", "red", "magenta", "yellow", "cyan", "green", "gray", "blue" };
            //1,4,6,2,4,7,2,5,7,3,5

            var width = 24;

            var sb = new StringBuilder();

            var top = 1;
            for (int i = 0; i < width; i++)
            {
                var scope = new Scope(i * 1.5, -(i % 2) * 0.5 * Math.Sqrt(3));
                var v = top;
                for (int j = 0; j < 8; j++)
                {
                    scope.AddNode(colors[v], 0, -j * Math.Sqrt(3), v.ToString());
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
        }
    }
}
