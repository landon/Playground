using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class Int32Usage
    {
        const UInt32 DeBruijnMultiplier = 0x077CB531U;
        static int[] DeBruijnLookup = new int[32] { 0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9 };
        
        public static int LeastSignificantBit(this int x)
        {
            return DeBruijnLookup[unchecked((uint)(x & -x) * DeBruijnMultiplier >> 27)];
        }

        public static int GetAndClearLeastSignificantBit(ref int x)
        {
            var m = x & -x;
            x ^= m;
            return DeBruijnLookup[unchecked((uint)m * DeBruijnMultiplier >> 27)];
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
            {
                var lsb = x & (0 - x);
                onBits.Add(DeBruijnLookup[unchecked(lsb * DeBruijnMultiplier >> 27)]);

                x ^= lsb;
            }

            return onBits;
        }
    }
}
