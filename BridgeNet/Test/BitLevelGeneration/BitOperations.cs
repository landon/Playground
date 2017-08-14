using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public static class BitOperations
    {
        public static ulong To_ulong(this IEnumerable<int> bits)
        {
            ulong x = 0;
            foreach (var bit in bits)
                x |= 1UL << bit;

            return x;
        }

        public static ushort To_ushort(this IEnumerable<int> bits)
        {
            ushort x = 0;
            foreach (var bit in bits)
                x |= (ushort)(1 << bit);

            return x;
        }

        public static byte To_byte(this IEnumerable<int> bits)
        {
            byte x = 0;
            foreach (var bit in bits)
                x |= (byte)(1 << bit);

            return x;
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
        
        public static ushort RightFillToMSB(this ushort x)
        {
            x |= (ushort)(x >> 1);
            x |= (ushort)(x >> 2);
            x |= (ushort)(x >> 4);
            x |= (ushort)(x >> 8);

            return x;
        }
        public static byte RightFillToMSB(this byte x)
        {
            x |= (byte)(x >> 1);
            x |= (byte)(x >> 2);
            x |= (byte)(x >> 4);

            return x;
        }
	}

    public static class BitUsage_uint
    {
        const uint DeBruijnMultiplier = 0x077CB531U;
        static int[] DeBruijnLookup = new int[32] { 0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9 };

        public static int Extract(this uint x)
        {
            return DeBruijnLookup[(x * DeBruijnMultiplier) >> 27];
        }

        public static int LeastSignificantBit(this uint x)
        {
            return DeBruijnLookup[((x & (~x + 1)) * DeBruijnMultiplier) >> 27];
        }

        public static int GetAndClearLeastSignificantBit(ref uint x)
        {
            var m = x & (~x + 1);
            x ^= m;
            return DeBruijnLookup[(m * DeBruijnMultiplier) >> 27];
        }

        public static int PopulationCount(this uint b)
        {
            int q = 0;
            while (b > 0)
            {
                q++;
                b &= b - 1;
            }
            return q;
        }
        public static List<int> ToSet(this uint x)
        {
            var onBits = new List<int>(10);
            while (x != 0)
                onBits.Add(GetAndClearLeastSignificantBit(ref x));

            return onBits;
        }
        public static uint RightFillToMSB(this uint x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;

            return x;
        }
        public static uint To_uint(this IEnumerable<int> bits)
        {
            uint x = 0;
            foreach (var bit in bits)
                x |= 1U << bit;

            return x;
        }

        public static uint Or(this uint[] colorGraph, int offset)
        {
            var result = 0U;
            for (int i = offset; i < colorGraph.Length; i++)
                result |= colorGraph[i];
            return result;
        }

        public static bool TrueForAllBitIndices(this uint x, Func<int, bool> predicate)
        {
            while (x != 0)
            {
                if (!predicate(GetAndClearLeastSignificantBit(ref x)))
                    return false;
            }

            return true;
        }
    }

    public static class BitUsage_long
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

        public static int Extract(this long x)
        {
            return DeBruijnLookup[((UInt64)x * DeBruijnMultiplier) >> 58];
        }

        public static int LeastSignificantBit(this long x)
        {
            return DeBruijnLookup[unchecked((UInt64)(x & -x) * DeBruijnMultiplier >> 58)];
        }

        public static int GetAndClearLeastSignificantBit(ref long x)
        {
            var m = x & -x;
            x ^= m;
            return DeBruijnLookup[((UInt64)m * DeBruijnMultiplier) >> 58];
        }

        public static int PopulationCount(this long b)
        {
            int q = 0;
            while (b > 0)
            {
                q++;
                b &= b - 1;
            }
            return q;
        }

        public static int PopulationCountDense(this long x)
        {
            x = x - ((x >> 1) & 0x5555555555555555L);
            x = (x & 0x3333333333333333L) + ((x >> 2) & 0x3333333333333333L);
            x = (x + (x >> 4)) & 0x0f0f0f0f0f0f0f0fL;
            x = (x * 0x0101010101010101L) >> 56;

            return (int)x;
        }

        public static List<int> ToSet(long x)
        {
            var onBits = new List<int>(10);
            while (x != 0)
                onBits.Add(GetAndClearLeastSignificantBit(ref x));

            return onBits;
        }
        public static long RightFillToMSB(this long x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;

            return x;
        }
        public static long To_long(this IEnumerable<int> bits)
        {
            long x = 0;
            foreach (var bit in bits)
                x |= 1L << bit;

            return x;
        }

        public static long Or(this long[] colorGraph, int offset)
        {
            var result = 0L;
            for (int i = offset; i < colorGraph.Length; i++)
                result |= colorGraph[i];
            return result;
        }

        public static bool TrueForAllBitIndices(this long x, Func<int, bool> predicate)
        {
            while (x != 0)
            {
                if (!predicate(GetAndClearLeastSignificantBit(ref x)))
                    return false;
            }

            return true;
        }
    }
}
