using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using Graphs;

namespace GraphsCore
{
    public static class GraphIO
    {
        public static string ToAdjacencyMatrix(this List<int> edgeWeights, bool isDirected = false)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * edgeWeights.Count)) / 2);
            var m = new char[n, n];

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var ew = edgeWeights[k];
                    if (isDirected)
                    {
                        m[i, j] = m[j, i] = '0';

                        if (ew > 0)
                            m[i, j] = '1';
                        else if (ew < 0)
                            m[j, i] = '1';
                    }
                    else
                    {
                        m[i, j] = m[j, i] = ew != 0 ? '1' : '0';
                    }

                    k++;
                }
            }

            for (int i = 0; i < n; i++)
                m[i, i] = '0';

            var s = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    s += m[i, j];
                s += Environment.NewLine;
            }

            return s;
        }

        //public static Graphs.Graph GraphFromGraph6(string graph6)
        //{
        //    var w = GetEdgeWeights(graph6);
        //    var h = new Choosability.Graph(w);

        //    var g = new Graphs.Graph(h, h.GetSpringsLayout(12), false);
        //    g.Name = graph6;
        //    return g;
        //}

        //public static Graphs.Graph GraphFromEdgeWeightString(string s)
        //{
        //    var isDirected = s.Contains("-1");

        //    var parts = s.Split(' ');
        //    var edgeWeights = parts.Where(p => !p.StartsWith("[")).Select(x => int.Parse(x)).ToList();

        //    List<int> vertexWeights = null;
        //    var vwp = parts.FirstOrDefault(p => p.StartsWith("["));
        //    if (vwp != null)
        //        vertexWeights = vwp.Trim('[').Trim(']').Split(',').Select(x => int.Parse(x)).ToList();

        //    var h = new Choosability.Graph(edgeWeights, vertexWeights);

        //    return new Graphs.Graph(h, h.GetSpringsLayout(12), isDirected);
        //}

        //public static List<int> GetEdgeWeights(string graph6)
        //{
        //    var chars = graph6.ToCharArray();

        //    var bytes = UTF8Encoding.UTF8.GetBytes(chars);

        //    var N = bytes[0] - 63;
        //    var h = N * (N - 1) / 2;
        //    var w = bytes.Skip(1).SelectMany(b => Low6((byte)(b - 63))).Take(h).ToList();

        //    var p = RowToColumnPermutation(N).ToList();
        //    var wp = new List<int>(h);

        //    for (int i = 0; i < p.Count; i++)
        //        wp.Add(w[p[i]]);

        //    return wp;
        //}

        //public static bool[,] GetAdjacencyMatrix(string adjacencyListString)
        //{
        //    if (string.IsNullOrEmpty(adjacencyListString))
        //        return null;

        //    try
        //    {
        //        var lines = adjacencyListString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        //        var adjacencyList = new Dictionary<int, List<int>>();
        //        foreach (var line in lines.Where(l => l.Contains(":")))
        //        {
        //            var chunks = line.Split(':', ',');

        //            adjacencyList[int.Parse(chunks[0])] = chunks.Skip(1).Select(c => int.Parse(c)).ToList();
        //        }

        //        var min = adjacencyList.Min(kvp => kvp.Key);
        //        var max = adjacencyList.Max(kvp => kvp.Key);
        //        var N = max + 1 - min;

        //        var adjacencyMatrix = new bool[N, N];

        //        foreach (var kvp in adjacencyList)
        //            foreach (var neighbor in kvp.Value)
        //                adjacencyMatrix[kvp.Key - min, neighbor - min] = true;

        //        return adjacencyMatrix;
        //    }
        //    catch { }

        //    return null;
        //}

        public static string ToGraph6(this Graph g)
        {
            return g.GetEdgeWeights().ToGraph6();
        }

        const string ASCII = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        public static string ToGraph6(this List<int> w)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * w.Count)) / 2);
            if (n > 62)
                throw new NotImplementedException("i have yet to write/read graph6 files with more than 62 vertices.");

            var p = RowToColumnPermutation(n).ToList();
            var wp = new List<int>(w.Count);

            for (int i = 0; i < p.Count; i++)
                wp.Add(Math.Abs(w[p.IndexOf(i)]));

            while (wp.Count % 6 != 0)
                wp.Add(0);

            var bb = wp.Batch<int, byte>(6, bits => (byte)(bits.Reverse().Index().Select(pair => pair.Value << pair.Key).Sum() + 63)).Prepend((byte)(n + 63)).ToArray();
            return string.Join("", bb.Select(b => ASCII[b - 32].ToString()));
        }

        static IEnumerable<int> RowToColumnPermutation(int n)
        {
            var x = 0;
            for (int j = 0; j < n - 1; j++)
            {
                var y = x;
                for (int i = 0; i < n - 1 - j; i++)
                {
                    yield return y;

                    y += 1 + j + i;
                }

                x += 2 + j;
            }
        }

        static IEnumerable<int> Low6(byte b)
        {
            for (int i = 5; i >= 0; i--)
                yield return (b & (1 << i)) >> i;
        }
    }
}
