using Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GraphsCore
{
    public static class CompactSerializer
    {
        const int Scale = 10000;
        const string Prefix = "webgraph:";

        public static bool LooksLikeASerializedGraph(string s)
        {
            return !string.IsNullOrEmpty(s) && s.StartsWith(Prefix);
        }

        //public static string Serialize(Graph g)
        //{
        //    var bytes = SerializeToByteArray(g);
        //    var compressed = QuickLZ.compress(bytes);
        //    var Ascii85 = new Ascii85();
        //    return Prefix + Ascii85.Encode(compressed);
        //}

        //static byte[] SerializeToByteArray(Graph g)
        //{
        //    using (var m = new MemoryStream())
        //    using (var bw = new BinaryWriter(m, Encoding.UTF8))
        //    {
        //        bw.Write((byte)g.Vertices.Count);
        //        foreach (var v in g.Vertices)
        //        {
        //            bw.Write((UInt16)Math.Round(v.X * Scale));
        //            bw.Write((UInt16)Math.Round(v.Y * Scale));
        //            bw.Write((UInt16)Math.Round(v.Padding * Scale));
        //            bw.Write(v.Label);
        //            bw.Write(v.Style ?? "");
        //        }

        //        bw.Write((UInt16)g.Edges.Count);
        //        foreach (var e in g.Edges)
        //        {
        //            bw.Write((byte)(g.Vertices.IndexOf(e.V1)));
        //            bw.Write((byte)(g.Vertices.IndexOf(e.V2)));
        //            bw.Write((byte)e.Multiplicity);
        //            bw.Write((byte)e.Orientation);
        //            bw.Write((UInt16)Math.Round(e.Thickness * 100));
        //            var label = e.Label ?? "";
        //            var hackedStyle = (e.Style ?? "") + "~~|~|~~" + label;
        //            bw.Write(hackedStyle);
        //        }

        //        return m.ToArray();
        //    }
        //}

        //public static Graph Deserialize(string s)
        //{
        //    try
        //    {
        //        var Ascii85 = new Ascii85();
        //        var compressed = Ascii85.Decode(s.Substring(Prefix.Length));
        //        var bytes = QuickLZ.decompress(compressed);
        //        return DeserializeFromByteArray(bytes);
        //    }
        //    catch { }

        //    return null;
        //}

        //static Graph DeserializeFromByteArray(byte[] bytes)
        //{
        //    using(var m = new MemoryStream(bytes))
        //    using (var br = new BinaryReader(m, Encoding.UTF8))
        //    {
        //        var n = br.ReadByte();
        //        var vertices = Enumerable.Range(0, n).Select(_ => new Vertex((double)br.ReadUInt16() / Scale, (double)br.ReadUInt16() / Scale) { Padding = (float)br.ReadUInt16() / Scale, Label = br.ReadString(), Style = br.ReadString() }).ToList();

        //        var e = br.ReadUInt16();
        //        var edges = Enumerable.Range(0, e).Select(_ => 
        //            new Edge(vertices[br.ReadByte()], vertices[br.ReadByte()]) { Multiplicity = br.ReadByte(), Orientation = (Edge.Orientations)br.ReadByte(), Thickness = (float)br.ReadUInt16() / 100, Style = br.ReadString() }).ToList();

        //        return new Graph(vertices, edges);
        //    }
        //}
    }
}
