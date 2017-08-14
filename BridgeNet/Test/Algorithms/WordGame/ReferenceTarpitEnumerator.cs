using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Algorithms.Utility;

namespace Algorithms.WordGame
{
    public class ReferenceTarpitEnumerator : TarpitEnumerator
    {
        List<string> _words;
        AccessibilityChecker _accessibilityChecker;

        public ReferenceTarpitEnumerator(int n)
        {
            _words = new WordEnumerator(Alphabet).EnumerateWords(n).ToList();
            _accessibilityChecker = new AccessibilityChecker(Alphabet);
        }

        public override IEnumerable<List<string>> EnumerateMinimalTarpits()
        {
            var seen = new HashSet<string>();
            return EnumerateMinimalTarpitsIn(_words, seen);
        }

        IEnumerable<List<string>> EnumerateMinimalTarpitsIn(List<string> S, HashSet<string> seen)
        {
            var excluded = false;
            foreach (var T in S.Select(w => S.Except(new[] { w }).ToList()))
            {
                var W = RunEscape(T);
                if (W.Count <= 0)
                    continue;

                excluded = true;

                var key = GenerateSimpleKey(W);
                if (seen.Contains(key))
                    continue;

                seen.Add(key);
                foreach (var TP in EnumerateMinimalTarpitsIn(W, seen))
                    yield return TP;
            }

            if (!excluded)
                yield return S.OrderBy(s => s).ToList();
        }

        List<string> RunEscape(List<string> S)
        {
            var T = S.ToList();
            while (true)
            {
                var R = _words.Difference(T);
                if (T.RemoveAll(w => _accessibilityChecker.IsAccessible(w, R), (x) => { }) <= 0)
                    break;
            }

            return T;
        }

        string GenerateSimpleKey(List<string> S)
        {
            return string.Join(",", S.OrderBy(s => s));
        }
    }
}
