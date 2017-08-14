using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Utility
{
    public class SequenceGeneralizer<T>
    {
        int _length;
        List<T> _alphabet;
        List<Matcher> _matchers = new List<Matcher>();
        Dictionary<int, List<int>> _generalizers;

        public SequenceGeneralizer(int length, List<T> alphabet)
        {
            _length = length;
            _alphabet = alphabet;

            foreach (var t in alphabet)
                AddMatcher(t.ToString(), tt => t.Equals(tt));

            AddMatcher("*", tt => true);
        }

        public void AddMatcher(string name, Func<T, bool> match)
        {
            _matchers.Add(new Matcher() { Name = name, Match = match });
        }

        VectorComparer _vectorComparer = new VectorComparer();
        public List<List<Matcher>> Generalize(List<List<T>> examples, List<List<T>> nonExamples, bool mostGeneral = false)
        {
            BuildMatcherLattice();

            var possibles = new List<List<int>>();
            foreach (var example in examples)
            {
                var asMatcherIndices = example.Select(t => _matchers.FirstIndex(m => m.Name == t.ToString())).ToList();
                possibles.AddRange(asMatcherIndices.Select(i => _generalizers[i]).CartesianProduct().Select(v => v.ToList()));
            }

            possibles = possibles.Distinct(_vectorComparer).ToList();

            foreach (var nonExample in nonExamples)
            {
                var asMatcherIndices = nonExample.Select(t => _matchers.FirstIndex(m => m.Name == t.ToString())).ToList();
                possibles = possibles.Except(asMatcherIndices.Select(i => _generalizers[i]).CartesianProduct().Select(v => v.ToList()), _vectorComparer).ToList();
            }

            var generalizations = new List<List<int>>();
            var remaining = examples.ToList();
            while (remaining.Count > 0)
            {
                var ordered = possibles.Select(pp => new {Vector = pp, Count = remaining.Count(r => IsMatch(r, pp))}).OrderByDescending(xx => xx.Count).ToList();
                var maxers = ordered.TakeWhile(xx => xx.Count == ordered[0].Count).OrderByDescending(xx => xx.Vector.Aggregate((total, x) =>
                    {
                        if (_matchers[x].Name == "*")
                            return total * _alphabet.Count;
                        if (_matchers[x].Name.StartsWith("!"))
                            return total * (_alphabet.Count - 1);

                        return total;
                    }));

                var best = mostGeneral ? maxers.First().Vector : maxers.Last().Vector;
                
                generalizations.Add(best);
                remaining.RemoveAll(r => IsMatch(r, best), (x) => { });
            }

            return generalizations.Select(l => l.Select(i => _matchers[i]).ToList()).ToList();
        }

        public class VectorComparer : IEqualityComparer<List<int>>
        {
            public bool Equals(List<int> x, List<int> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<int> l)
            {
                unchecked
                {
                    int hash = 17;
                    foreach (var x in l)
                        hash = hash * 23 + x;

                    return hash;
                }
            }
        }

        void BuildMatcherLattice()
        {
            _generalizers = new Dictionary<int, List<int>>();
            for(int i = 0; i < _matchers.Count; i++)
            {
                _generalizers[i] = new List<int>();
                var matches = _alphabet.Where(a => _matchers[i].Match(a)).ToList();

                for(int j = 0; j < _matchers.Count; j++)
                {
                    if (matches.SubsetEqual(_alphabet.Where(a => _matchers[j].Match(a)).ToList()))
                        _generalizers[i].Add(j);
                }
            }
        }

        bool IsMatch(List<T> vector, List<int> m)
        {
            for (int i = 0; i < vector.Count; i++)
            {
                if (!_matchers[m[i]].Match(vector[i]))
                    return false;
            }

            return true;
        }

        public class Matcher
        {
            public string Name;
            public Func<T, bool> Match;
        }
    }
}
