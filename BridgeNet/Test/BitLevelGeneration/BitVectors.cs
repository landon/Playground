
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public static class BitVectors_ulong
    {
        public static List<int> FromBitVector(this List<ulong> n) 
        {
            var v = new List<int>(64);
            for (int i = 0; i < 64; i++)
            {
                var b = (ulong)(1UL << i);
                int m = 0;
                for (int j = 0; j < n.Count; j++)
                {
                    if ((b & n[j]) != 0)
                        m += 1 << j;
                }

                v.Add(m);
            }

            return v;
        }

        public static List<ulong> ToBitVector(IEnumerable<int> w)
        {
            var v = w.ToList();
            var n = new List<ulong>();
            while (true)
            {
                ulong m = 0;
                var zero = true;
                for (int i = 0; i < v.Count; i++)
                {
                    if (v[i] != 0)
                        zero = false;

                    if ((v[i] & 1) != 0)
                        m |= (ulong)(1UL << i);

                    v[i] >>= 1;
                }

                n.Add(m);

                if (zero)
                    break;
            }

            return n;
        }

        public static void Increment(this List<ulong> n, ulong m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (ulong)(m & n[i]);
                var t2 = (ulong)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static void Decrement(this List<ulong> n, ulong m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (ulong)(m & ~n[i]);
                var t2 = (ulong)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static ulong Zeroes(this List<ulong> n)
        {
            ulong m = 0;
            for (int i = 0; i < n.Count; i++)
                m |= n[i];

            return (ulong)~m;
        }

        public static ulong GreaterThan(this List<ulong> n, List<ulong> k)
        {
            ulong a = 0;
            ulong b = 0;
            for (int i = Math.Max(n.Count, k.Count) - 1; i >= 0; i--)
            {
                if (i >= k.Count)
                    a |= n[i];
                else if (i >= n.Count)
                    b |= k[i];
                else
                {
                    a |= (ulong)(~b & n[i] & ~k[i]);
                    b |= (ulong)(~a & ~n[i] & k[i]);
                }
            }

            return (ulong)(a & ~b);
        }
    }
}
namespace BitLevelGeneration
{
    public static class BitVectors_long
    {
        public static List<int> FromBitVector(this List<long> n) 
        {
            var v = new List<int>(64);
            for (int i = 0; i < 64; i++)
            {
                var b = (long)(1L << i);
                int m = 0;
                for (int j = 0; j < n.Count; j++)
                {
                    if ((b & n[j]) != 0)
                        m += 1 << j;
                }

                v.Add(m);
            }

            return v;
        }

        public static List<long> ToBitVector(IEnumerable<int> w)
        {
            var v = w.ToList();
            var n = new List<long>();
            while (true)
            {
                long m = 0;
                var zero = true;
                for (int i = 0; i < v.Count; i++)
                {
                    if (v[i] != 0)
                        zero = false;

                    if ((v[i] & 1) != 0)
                        m |= (long)(1L << i);

                    v[i] >>= 1;
                }

                n.Add(m);

                if (zero)
                    break;
            }

            return n;
        }

        public static void Increment(this List<long> n, long m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (long)(m & n[i]);
                var t2 = (long)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static void Decrement(this List<long> n, long m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (long)(m & ~n[i]);
                var t2 = (long)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static long Zeroes(this List<long> n)
        {
            long m = 0;
            for (int i = 0; i < n.Count; i++)
                m |= n[i];

            return (long)~m;
        }

        public static long GreaterThan(this List<long> n, List<long> k)
        {
            long a = 0;
            long b = 0;
            for (int i = Math.Max(n.Count, k.Count) - 1; i >= 0; i--)
            {
                if (i >= k.Count)
                    a |= n[i];
                else if (i >= n.Count)
                    b |= k[i];
                else
                {
                    a |= (long)(~b & n[i] & ~k[i]);
                    b |= (long)(~a & ~n[i] & k[i]);
                }
            }

            return (long)(a & ~b);
        }
    }
}
namespace BitLevelGeneration
{
    public static class BitVectors_uint
    {
        public static List<int> FromBitVector(this List<uint> n) 
        {
            var v = new List<int>(32);
            for (int i = 0; i < 32; i++)
            {
                var b = (uint)(1U << i);
                int m = 0;
                for (int j = 0; j < n.Count; j++)
                {
                    if ((b & n[j]) != 0)
                        m += 1 << j;
                }

                v.Add(m);
            }

            return v;
        }

        public static List<uint> ToBitVector(IEnumerable<int> w)
        {
            var v = w.ToList();
            var n = new List<uint>();
            while (true)
            {
                uint m = 0;
                var zero = true;
                for (int i = 0; i < v.Count; i++)
                {
                    if (v[i] != 0)
                        zero = false;

                    if ((v[i] & 1) != 0)
                        m |= (uint)(1U << i);

                    v[i] >>= 1;
                }

                n.Add(m);

                if (zero)
                    break;
            }

            return n;
        }

        public static void Increment(this List<uint> n, uint m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (uint)(m & n[i]);
                var t2 = (uint)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static void Decrement(this List<uint> n, uint m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (uint)(m & ~n[i]);
                var t2 = (uint)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static uint Zeroes(this List<uint> n)
        {
            uint m = 0;
            for (int i = 0; i < n.Count; i++)
                m |= n[i];

            return (uint)~m;
        }

        public static uint GreaterThan(this List<uint> n, List<uint> k)
        {
            uint a = 0;
            uint b = 0;
            for (int i = Math.Max(n.Count, k.Count) - 1; i >= 0; i--)
            {
                if (i >= k.Count)
                    a |= n[i];
                else if (i >= n.Count)
                    b |= k[i];
                else
                {
                    a |= (uint)(~b & n[i] & ~k[i]);
                    b |= (uint)(~a & ~n[i] & k[i]);
                }
            }

            return (uint)(a & ~b);
        }
    }
}
namespace BitLevelGeneration
{
    public static class BitVectors_ushort
    {
        public static List<int> FromBitVector(this List<ushort> n) 
        {
            var v = new List<int>(16);
            for (int i = 0; i < 16; i++)
            {
                var b = (ushort)(1 << i);
                int m = 0;
                for (int j = 0; j < n.Count; j++)
                {
                    if ((b & n[j]) != 0)
                        m += 1 << j;
                }

                v.Add(m);
            }

            return v;
        }

        public static List<ushort> ToBitVector(IEnumerable<int> w)
        {
            var v = w.ToList();
            var n = new List<ushort>();
            while (true)
            {
                ushort m = 0;
                var zero = true;
                for (int i = 0; i < v.Count; i++)
                {
                    if (v[i] != 0)
                        zero = false;

                    if ((v[i] & 1) != 0)
                        m |= (ushort)(1 << i);

                    v[i] >>= 1;
                }

                n.Add(m);

                if (zero)
                    break;
            }

            return n;
        }

        public static void Increment(this List<ushort> n, ushort m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (ushort)(m & n[i]);
                var t2 = (ushort)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static void Decrement(this List<ushort> n, ushort m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (ushort)(m & ~n[i]);
                var t2 = (ushort)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static ushort Zeroes(this List<ushort> n)
        {
            ushort m = 0;
            for (int i = 0; i < n.Count; i++)
                m |= n[i];

            return (ushort)~m;
        }

        public static ushort GreaterThan(this List<ushort> n, List<ushort> k)
        {
            ushort a = 0;
            ushort b = 0;
            for (int i = Math.Max(n.Count, k.Count) - 1; i >= 0; i--)
            {
                if (i >= k.Count)
                    a |= n[i];
                else if (i >= n.Count)
                    b |= k[i];
                else
                {
                    a |= (ushort)(~b & n[i] & ~k[i]);
                    b |= (ushort)(~a & ~n[i] & k[i]);
                }
            }

            return (ushort)(a & ~b);
        }
    }
}
namespace BitLevelGeneration
{
    public static class BitVectors_byte
    {
        public static List<int> FromBitVector(this List<byte> n) 
        {
            var v = new List<int>(8);
            for (int i = 0; i < 8; i++)
            {
                var b = (byte)(1 << i);
                int m = 0;
                for (int j = 0; j < n.Count; j++)
                {
                    if ((b & n[j]) != 0)
                        m += 1 << j;
                }

                v.Add(m);
            }

            return v;
        }

        public static List<byte> ToBitVector(IEnumerable<int> w)
        {
            var v = w.ToList();
            var n = new List<byte>();
            while (true)
            {
                byte m = 0;
                var zero = true;
                for (int i = 0; i < v.Count; i++)
                {
                    if (v[i] != 0)
                        zero = false;

                    if ((v[i] & 1) != 0)
                        m |= (byte)(1 << i);

                    v[i] >>= 1;
                }

                n.Add(m);

                if (zero)
                    break;
            }

            return n;
        }

        public static void Increment(this List<byte> n, byte m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (byte)(m & n[i]);
                var t2 = (byte)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static void Decrement(this List<byte> n, byte m)
        {
            var i = 0;
            while (m != 0)
            {
                var t1 = (byte)(m & ~n[i]);
                var t2 = (byte)(m ^ n[i]);

                m = t1;
                n[i] = t2;

                i++;
            }
        }

        public static byte Zeroes(this List<byte> n)
        {
            byte m = 0;
            for (int i = 0; i < n.Count; i++)
                m |= n[i];

            return (byte)~m;
        }

        public static byte GreaterThan(this List<byte> n, List<byte> k)
        {
            byte a = 0;
            byte b = 0;
            for (int i = Math.Max(n.Count, k.Count) - 1; i >= 0; i--)
            {
                if (i >= k.Count)
                    a |= n[i];
                else if (i >= n.Count)
                    b |= k[i];
                else
                {
                    a |= (byte)(~b & n[i] & ~k[i]);
                    b |= (byte)(~a & ~n[i] & k[i]);
                }
            }

            return (byte)(a & ~b);
        }
    }
}

