using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public static class BoardLexifier
    {
        static Dictionary<string, Tuple<string, Permutation>> _lookup = new Dictionary<string, Tuple<string, Permutation>>();
        public static string ToListStringInLexOrder(long[] ss, out Permutation pp, int maxPot = -1)
        {
            var lists = ss.Select(l => l.ToSet()).ToList();
            var pot = maxPot;
            if (pot <= 0)
                pot = lists.SelectMany(l => l).Distinct().Count();

            var stacks = lists.Where(s => s.Count <= pot).ToList();
            return ToLexOrder(string.Join("|", stacks.Select(s => string.Join("", s.OrderBy(x => x)))), out pp, maxPot);
        }

        static string ToLexOrder(string stacksString, out Permutation pp, int maxPot)
        {
            var key = stacksString + ";" + maxPot;
            Tuple<string, Permutation> tup;
            if (_lookup.TryGetValue(key, out tup))
            {
                pp = tup.Item2;
                return tup.Item1;
            }

            pp = null;
            string lexSmallest;
            var stacks = stacksString.Split('|').Select(s => s.ToCharArray().Select(c => int.Parse(c.ToString())).ToList()).ToList();
            lexSmallest = stacksString;

            var pot = stacks.SelectMany(l => l).Distinct().Count();
            if (maxPot > 0)
                pot = maxPot;

            foreach (var p in Permutation.EnumerateAll(pot))
            {
                var permutedString = string.Join("|", stacks.Select(s => string.Join("", s.Select(a => p[a]).OrderBy(x => x))));
                if (permutedString.CompareTo(lexSmallest) <= 0)
                {
                    lexSmallest = permutedString;
                    pp = p;
                }
            }

            _lookup[key] = new Tuple<string, Permutation>(lexSmallest, pp);
            return lexSmallest;
        }
    }
}
