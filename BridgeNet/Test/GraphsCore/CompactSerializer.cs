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

        public static string Serialize(Graph g)
        {
            var bytes = SerializeToByteArray(g);
            var compressed = QuickLZ.compress(bytes);
            var Ascii85 = new Ascii85();
            return Prefix + Ascii85.Encode(compressed);
        }

        static byte[] SerializeToByteArray(Graph g)
        {
            var bytes = new List<byte>();
            bytes.Add((byte)g.Vertices.Count);
            foreach (var v in g.Vertices)
            {
                bytes.AddRange(BitConverter.GetBytes((UInt16)Math.Round(v.X * Scale)));
                bytes.AddRange(BitConverter.GetBytes((UInt16)Math.Round(v.Y * Scale)));
                bytes.AddRange(BitConverter.GetBytes((UInt16)Math.Round(v.Padding * Scale)));
                bytes.AddRange(GetBytes(v.Label));
                bytes.AddRange(GetBytes(v.Style ?? ""));
            }

            bytes.AddRange(BitConverter.GetBytes((UInt16)g.Edges.Count));
            foreach (var e in g.Edges)
            {
                bytes.Add((byte)(g.Vertices.IndexOf(e.V1)));
                bytes.Add((byte)(g.Vertices.IndexOf(e.V2)));
                bytes.Add((byte)e.Multiplicity);
                bytes.Add((byte)e.Orientation);
                bytes.AddRange(BitConverter.GetBytes((UInt16)Math.Round(e.Thickness * 100)));

                var label = e.Label ?? "";
                var hackedStyle = (e.Style ?? "") + "~~|~|~~" + label;
                bytes.AddRange(GetBytes(hackedStyle));
            }

            return bytes.ToArray();
        }

        public static Graph Deserialize(string s)
        {
            try
            {
                var Ascii85 = new Ascii85();
                var compressed = Ascii85.Decode(s.Substring(Prefix.Length));
                var bytes = QuickLZ.decompress(compressed);
                return DeserializeFromByteArray(bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        static Graph DeserializeFromByteArray(byte[] bytes)
        {
            var offset = 0;
            var n = ReadByte(bytes, ref offset);
            var vertices = new List<Vertex>();
            for (int i = 0; i < n; i++)
            {
                var x = (double)ReadUInt16(bytes, ref offset) / Scale;
                var y = (double)ReadUInt16(bytes, ref offset) / Scale;
                var padding = (float)ReadUInt16(bytes, ref offset) / Scale;
                var label = ReadString(bytes, ref offset);
                var style = ReadString(bytes, ref offset);
                vertices.Add(new Vertex(x, y) { Padding = padding, Label = label, Style = style });
            }

            var e = ReadUInt16(bytes, ref offset);
            var edges = new List<Edge>();
            for (int i = 0; i < e; i++)
            {
                var v1 = ReadByte(bytes, ref offset);
                var v2 = ReadByte(bytes, ref offset);
                var multiplicity = ReadByte(bytes, ref offset);
                var orientation = (Edge.Orientations)ReadByte(bytes, ref offset);
                var thickness = (float)ReadUInt16(bytes, ref offset) / 100;
                var style = ReadString(bytes, ref offset);

                edges.Add(new Edge(vertices[v1], vertices[v2]) { Multiplicity = multiplicity, Orientation = orientation, Thickness = thickness, Style = style });
            }

            return new Graph(vertices, edges);
        }

        static byte ReadByte(byte[] bytes, ref int offset)
        {
            return bytes[offset++];
        }

        static UInt16 ReadUInt16(byte[] bytes, ref int offset)
        {
            var x = BitConverter.ToUInt16(bytes, offset);
            offset += 2;
            return x;
        }

        static UInt32 ReadUInt32(byte[] bytes, ref int offset)
        {
            var x = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            return x;
        }

        static byte[] Get7BitEncodedInt(int value)
        {
            var bytes = new List<byte>();
            uint num = (uint)value;
            while (num >= 128U)
            {
                bytes.Add((byte)(num | 128U));
                num >>= 7;
            }
            bytes.Add((byte)num);

            return bytes.ToArray();
        }

        static int Read7BitEncodedInt(byte[] bytes, ref int offset)
        {
            int num1 = 0;
            int num2 = 0;
            while (num2 != 35)
            {
                byte num3 = ReadByte(bytes, ref offset);
                num1 |= ((int)num3 & (int)sbyte.MaxValue) << num2;
                num2 += 7;
                if (((int)num3 & 128) == 0)
                    return num1;
            }
            throw new FormatException("bad 7-bit encoded int");
        }

        static byte[] GetBytes(string s)
        {
            var bytes = new List<byte>();
            foreach(var c in s.ToCharArray())
            {
                bytes.AddRange(BitConverter.GetBytes(c));
            }

            foreach (var b in ((IEnumerable<byte>)Get7BitEncodedInt(bytes.Count)).Reverse())
            {
                bytes.Insert(0, b);
            }

            return bytes.ToArray();
        }

        const string ASCII = " !\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        static string ReadString(byte[] bytes, ref int offset)
        {
            var length = Read7BitEncodedInt(bytes, ref offset);

            var s = "";
            while(length-- > 0) s += ASCII[bytes[offset++] - 32];
            return s;
        }
    }
}
