using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.WordGame
{
    public abstract class TarpitEnumerator
    {
        public static readonly List<char> Alphabet = new List<char>() { 'x', 'y', 'z' };

        public static bool IsPermutationClosed(List<string> S)
        {
            return EnumerateAlphabetPermutations(S).Select(T => string.Join(",", T.OrderBy(s => s))).Distinct().Count() == 1;
        }

        public static List<string> RemovePermutationRedundancies(List<string> S)
        {
            var hash = new HashSet<string>();
            var permutedLists = EnumerateAlphabetPermutations(S).ToList();
            for (int i = 0; i < S.Count; i++)
            {
                hash.Add(permutedLists.Select(l => l[i]).OrderBy(s => s).First());
            }

            return hash.ToList();
        }

        public static List<string> ReorderAlphabetByOccurenceRate(List<string> S)
        {
            return S.Select(s =>
            {
                var chars = s.ToCharArray();
                var p = Alphabet.Select(c => new { C = c, Rate = chars.Count(cc => cc == c) }).OrderByDescending(d => d.Rate).ToList();

                return s.Replace(p[0].C, 'X').Replace(p[1].C, 'Y').Replace(p[2].C, 'Z').ToLower();
                
            }).ToList();
        }

        public static IEnumerable<List<string>> EnumerateAlphabetPermutations(List<string> S)
        {
            var uppered = S.Select(s => s.ToUpper()).ToList();
            foreach (var permutation in Permutation.EnumerateAll(3))
            {
                var p = permutation.Apply(Alphabet);
                yield return uppered.Select(s => s.Replace('X', p[0]).Replace('Y', p[1]).Replace('Z', p[2])).ToList();
            }
        }

        public abstract IEnumerable<List<string>> EnumerateMinimalTarpits();
        public virtual void GenerateMinimalTarpits(Action<List<string>> foundTarpit) { }
    }
}
