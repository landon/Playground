using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class Int64Usage
    {
        const UInt64 DeBruijnMultiplier = 0x07EDD5E59A4E28C2;
        static int[] DeBruijnLookup = {
                                           63,  0, 58,  1, 59, 47, 53,  2,
                                           60, 39, 48, 27, 54, 33, 42,  3,
                                           61, 51, 37, 40, 49, 18, 28, 20,
                                           55, 30, 34, 11, 43, 14, 22,  4,
                                           62, 57, 46, 52, 38, 26, 32, 41,
                                           50, 36, 17, 19, 29, 10, 13, 21,
                                           56, 45, 25, 31, 35, 16,  9, 12,
                                           44, 24, 15,  8, 23,  7,  6,  5
                                       };

        static readonly Int64[] BitMask = new Int64[64];
        static readonly Int64[] NotBitMask = new Int64[64];

        static Int64Usage()
        {
            foreach (int bit in Enumerable.Range(0, 64))
            {
                BitMask[bit] = ((Int64)1) << bit;
                NotBitMask[bit] = ~(((Int64)1) << bit);
            }
        }

        public static Dictionary<long, long> GeneratePairMasks(int n)
        {
            var maskMap = new Dictionary<long, long>();

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    maskMap[BitMask[i] | BitMask[j]] = BitMask[j];
                }
            }

            return maskMap;
        }

        public static Int64 ClearBit(this Int64 x, int bit)
        {
            return x & NotBitMask[bit];
        }

        public static Int64 SetBit(this Int64 x, int bit)
        {
            return x | BitMask[bit];
        }

        public static bool IsBitSet(this Int64 x, int bit)
        {
            return (x & BitMask[bit]) != 0;
        }

        public static Int64 FlipBits(this Int64 x, int bit)
        {
            return x ^ BitMask[bit];
        }
        public static Int64 FlipBits(this Int64 x, int bit1, int bit2)
        {
            return x ^ (BitMask[bit1] | BitMask[bit2]);
        }
        public static Int64 FlipBits(this Int64 x, params int[] bits)
        {
            if (bits == null)
                return x;

            Int64 mask = 0;
            foreach (var bit in bits)
                mask |= BitMask[bit];

            return x ^ mask;
        }

        public static int LeastSignificantBit(this Int64 b)
        {
            return DeBruijnLookup[unchecked((UInt64)(b & -b) * DeBruijnMultiplier >> 58)];
        }
        public static int GetAndClearLeastSignificantBit(ref Int64 x)
        {
            var m = x & -x;
            x ^= m;
            return DeBruijnLookup[((UInt64)m * DeBruijnMultiplier) >> 58];
        }

        public static int PopulationCount(this Int64 b)
        {
            int q = 0;
            while (b > 0)
            {
                q++;
                b &= b - 1;
            }
            return q;
        }

        public static int PopulationCount(this ulong x)
        {
            const ulong k1 = 0x5555555555555555UL;
            const ulong k2 = 0x3333333333333333UL;
            const ulong k4 = 0x0f0f0f0f0f0f0f0fUL;
            x = x - ((x >> 1) & k1);
            x = (x & k2) + ((x >> 2) & k2);
            x = (x + (x >> 4)) & k4;
            x = (x * 0x0101010101010101UL) >> 56;
            return (int)x;
        }

        public static ulong RightFillToMSB(this ulong x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;

            return x;
        }

        public static string ToBitString(this Int64 x, int length = 64)
        {
            return string.Join("", Enumerable.Range(0, length).Select(bit => x.IsBitSet(bit) ? "1" : "0"));
        }

        public static List<int> ToSet(this Int64 x)
        {
            var onBits = new List<int>(10);
            while (x != 0)
                onBits.Add(GetAndClearLeastSignificantBit(ref x));

            return onBits;
        }

        public static List<int> ToSet(this UInt64 x)
        {
            var onBits = new List<int>(10);
            while (x != 0)
            {
                var lsb = x & (0 - x);
                onBits.Add(DeBruijnLookup[unchecked(lsb * DeBruijnMultiplier >> 58)]);

                x ^= lsb;
            }

            return onBits;
        }

        public static IEnumerable<int> EnumerateBits(this Int64 x)
        {
            while (x != 0)
            {
                var lsb = GetAndClearLeastSignificantBit(ref x);
                yield return lsb;
            }

        }

        public static long ToInt64(params int[] bits)
        {
            long total = 0;
            foreach (var bit in bits)
                total |= BitMask[bit];

            return total;
        }

        public static long ToInt64(this IEnumerable<int> set)
        {
            long total = 0;
            foreach (var bit in set)
                total |= BitMask[bit];

            return total;
        }
        public static ulong ToUInt64(this IEnumerable<int> set)
        {
            ulong total = 0;
            foreach (var bit in set)
                total |= 1UL << bit;

            return total;
        }

        public static List<List<int>> ExpandIntoLists(this List<Int64> xs)
        {
            return xs.Select(x => x.ToSet().ToList()).ToList();
        }

        public static string ToSetString(this Int64 x)
        {
            return x.ToSet().ToSetString();
        }

        public static string ToSetStringSmall(this Int64 x)
        {
            return string.Join("|", string.Join("", x.ToSet()));
        }

        public static bool AllSet(this Int64 x, int i)
        {
            return (x ^ (x + 1)).IsBitSet(i);
        }

        public static List<long> GetBits(this long x)
        {
            var setBits = new List<long>();

            while (x != 0)
            {
                long lsb = x & -x;

                setBits.Add(lsb);

                x ^= lsb;
            }

            return setBits;
        }

        public static List<ulong> GetBits(this ulong x)
        {
            var setBits = new List<ulong>(16);

            while (x != 0)
            {
                ulong lsb = x & (0 - x);
                setBits.Add(lsb);
                x ^= lsb;
            }

            return setBits;
        }

        public static string ToSetString(this IEnumerable<Int64> list)
        {
            return "{" + string.Join(", ", list.Select(l => l.ToSetString())) + "}";
        }

        public static string ToSetString(this IEnumerable<int> list)
        {
            return "{" + string.Join(", ", list) + "}";
        }
    }
}
