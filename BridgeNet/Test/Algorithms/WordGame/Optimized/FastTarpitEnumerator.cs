using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;

namespace Algorithms.WordGame.Optimized
{
    public class FastTarpitEnumerator : TarpitEnumerator
    {
        List<FastWord> _words;
        FastAccessibilityChecker _accessibilityChecker;
        static readonly List<FastWord> EmptyList = new List<FastWord>();

        public FastTarpitEnumerator(int n)
        {
            _words = new FastWordGenerator().GenerateWords(n);
            _accessibilityChecker = new FastAccessibilityChecker(n);
        }

        public override void GenerateMinimalTarpits(Action<List<string>> foundTarpit)
        {
            GenerateMinimalTarpitsFast(l => foundTarpit(l.Select(s => Wordify(s)).ToList()));
        }

        public void GenerateMinimalTarpitsFast(Action<List<FastWord>> foundTarpit)
        {
            GenerateMinimalTarpitsFastIn(_words, new List<FastWord>(), foundTarpit);
        }

        void GenerateMinimalTarpitsFastIn(List<FastWord> S, List<FastWord> mustHaves, Action<List<FastWord>> foundTarpit)
        {
            var excluded = false;

            var extendedMustHaves = mustHaves.ToList();
            var T = S.Except(mustHaves).ToList();

            foreach (var w in T)
            {
                var W = RunEscape(S.Except(new[] { w }), extendedMustHaves);

                if (W != null && W.Count > 0)
                {
                    excluded = true;
                    GenerateMinimalTarpitsFastIn(W, extendedMustHaves, foundTarpit);
                }

                extendedMustHaves.Add(w);    
            }

            if (!excluded)
            {
                foreach (var w in mustHaves)
                {
                    var W = RunEscape(S.Except(new[] { w }));
                    if (W.Count > 0)
                    {
                        excluded = true;
                        break;
                    }
                }

                if (!excluded)
                    foundTarpit(S);
            }
        }

        List<FastWord> RunEscape(IEnumerable<FastWord> S, List<FastWord> mustHaves)
        {
            var T = S.ToList();
            var R = new HashSet<FastWord>(_words.Except(T));
            while (true)
            {
                if (mustHaves.Any(w => _accessibilityChecker.IsAccessible(w, R)))
                    return null;

                var escapers = T.Except(mustHaves)
                                .Where(w => _accessibilityChecker.IsAccessible(w, R))
                                .ToList();

                foreach (var w in escapers)
                {
                    T.Remove(w);
                    R.Add(w);
                }

                if (escapers.Count <= 0)
                    break;
            }

            return T;
        }

        List<FastWord> RunEscape(IEnumerable<FastWord> S)
        {
            return RunEscape(S, EmptyList);
        }

        string Wordify(FastWord b)
        {
            var st = new long[b._stackCount];
            var traceBits = b._trace.Select(t => t.ToSet()).ToList();
            for (int c = 0; c < traceBits.Count; c++)
                foreach (var i in traceBits[c])
                    st[i] |= 1L << c;

            return string.Join("", st.Select(s =>
            {
                switch (s)
                {
                    case 1: return 'x';
                    case 2: return 'y';
                    case 4: return 'z';
                    default: return '?';
                }
            }));
        }

        #region enunmerable way
        public override IEnumerable<List<string>> EnumerateMinimalTarpits()
        {
            return EnumerateMinimalTarpitsFast().Select(list => list.Select(Wordify).ToList());
        }

        public IEnumerable<List<FastWord>> EnumerateMinimalTarpitsFast()
        {
            var explored = new HashSet<List<FastWord>>(new SortedListComparer());
            return EnumerateMinimalTarpitsFastIn(_words, explored, new List<FastWord>());
        }

        IEnumerable<List<FastWord>> EnumerateMinimalTarpitsFastIn(List<FastWord> S, HashSet<List<FastWord>> explored, List<FastWord> mustHaves)
        {
            var excluded = false;

            foreach (var w in S)
            {
                var W = RunEscape(S.Except(new[] { w }));
                if (W.Count <= 0)
                    continue;

                excluded = true;

                if (mustHaves.SubsetEqual(W) && !explored.Contains(W))
                {
                    foreach (var TP in EnumerateMinimalTarpitsFastIn(W, explored, mustHaves.ToList()))
                        yield return TP;
                }

                explored.Add(W);
                mustHaves.Add(w);
            }

            if (!excluded)
                yield return S.ToList();
        } 
        #endregion

        class SortedListComparer : IEqualityComparer<List<FastWord>>
        {
            public bool Equals(List<FastWord> x, List<FastWord> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<FastWord> list)
            {
                unchecked
                {
                    int hash = 19;
                    foreach (var x in list)
                        hash = hash * 31 + x.GetHashCode();

                    return hash;
                }
            }
        }
    }
}
