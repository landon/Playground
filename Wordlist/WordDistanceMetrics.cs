using System;
using System.Collections.Generic;
using System.Text;

namespace Wordlist
{
    public static class WordDistanceMetrics
    {
        public static int LevenshteinDistance(string a, string b)
        {
            var cache = new Dictionary<Tuple<int, int>, int>();
            return LevenshteinDistance(a, b, a.Length, b.Length, cache);
        }

        static int LevenshteinDistance(string a, string b, int al, int bl, Dictionary<Tuple<int, int>, int> cache)
        {
            if (al == 0)
                return bl;
            if (bl == 0)
                return al;

            int distance;
            var key = new Tuple<int, int>(al, bl);
            if (cache.TryGetValue(key, out distance))
                return distance;

            var x = LevenshteinDistance(a, b, al - 1, bl, cache) + 1;
            var y = LevenshteinDistance(a, b, al, bl - 1, cache) + 1;
            var z = LevenshteinDistance(a, b, al - 1, bl - 1, cache);
            if (a[al - 1] != b[bl - 1])
                z++;

            distance = Math.Min(x, Math.Min(y, z));
            cache[key] = distance;

            return distance;
        }
    }
}
