﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Choosability;
using Choosability.Utility;
using Graphs;
using GraphsCore;

namespace Maker
{
    class Program
    {
        const int N = 13;

        static void Main(string[] args)
        {
            var vertices = new FareySequence(N).ToList();
            var adjacent = new bool[vertices.Count, vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    adjacent[i, j] = adjacent[j, i] = Math.Abs(vertices[i].Top * vertices[j].Bottom - vertices[i].Bottom * vertices[j].Top) == 1.0;
                }
            }

            List<int> R = null;
            List<int> G = null;
            var g = new Choosability.Graph(adjacent);
            foreach (var red in g.EnumerateMaximalIndependentSets())
            {
                var bluegreen = g.Vertices.Except(red).ToList();
                var h = g.InducedSubgraph(bluegreen);
                if (!h.IsTwoColorable())
                    continue;
                R = red;
                foreach (var green in g.EnumerateMaximalIndependentSets(bluegreen))
                {
                    if (g.EdgesOn(bluegreen.Except(green).ToList()) <= 0)
                    {
                        G = green;
                        break;
                    }
                }

                if (G != null)
                    break;
            }

            using (var sw = new StreamWriter("Ford" + N + ".tex"))
            {
                sw.WriteLine(@"\documentclass{standalone}");
                sw.WriteLine(@"\usepackage{tikz}");
                sw.WriteLine(@"\begin{document}");
                sw.WriteLine(@"\begin{tikzpicture}");
                var circles = string.Join(Environment.NewLine, new FareySequence(N).Select(f => new FordCircle(f)).Select((f,i) => string.Format(@"\fill[{3}] ({0},{1}) circle [radius={2}];", f.X, f.Y, f.R, Fill(f.F, i, R, G))));
                sw.WriteLine(circles);
                sw.WriteLine(@"\end{tikzpicture}");
                sw.WriteLine(@"\end{document}");

                


                //var k = g.CliqueNumberBronKerbosch();
                //var gg = GraphIO.GraphFromEdgeWeightString(string.Join(" ", g.GetEdgeWeights()));
                //var s = CompactSerializer.Serialize(gg);
            }
        }

        static string Fill(Fraction f, int i, List<int> R, List<int> G)
        {
            var a = f.Top;
            var b = f.Bottom;

            if (R.Contains(i))
                return "red";
            else if (G.Contains(i))
                return "green";
            return "blue";
        }
    }
}
