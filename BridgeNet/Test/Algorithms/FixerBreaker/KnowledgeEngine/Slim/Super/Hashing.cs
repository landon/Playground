using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    static class Hashing
    {
        const int MaxLength = 32;
        static List<ulong> _m;

        static Hashing()
        {
            _m = new List<ulong>(MaxLength);

            var RNG = new Random(DateTime.Now.Millisecond);
            var b = new byte[8];

            for (int i = 0; i < MaxLength; i++)
            {
                RNG.NextBytes(b);
                _m.Add(BitConverter.ToUInt64(b, 0));
            }
        }

        public static int Hash(ulong[] list, int length)
        {
            ulong hash = list[0];

            for (int i = 1; i < length; i++)
                hash += _m[i - 1] * list[i];

            return (int)(hash >> 32);
        }
    }
}
