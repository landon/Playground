using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class Hypergraph<T>
        where T : IComparable
    {
        SortedListComparer _listComparer = new SortedListComparer();
        public HashSet<List<T>> E { get; private set; }

        public Hypergraph(List<List<T>> edges = null)
        {
            if (edges != null)
            {
                foreach (var e in edges)
                    e.Sort();
                E = new HashSet<List<T>>(edges, _listComparer);
            }
            else
                E = new HashSet<List<T>>(_listComparer);
        }

        public Hypergraph<T> Min()
        {
            var chainHeads = new List<List<T>>();

            foreach (var set in E)
            {
                var removed = chainHeads.RemoveAll(head => set.SubsetEqualSorted(head), (x) => { });
                if (removed > 0 || chainHeads.All(head => !head.SubsetEqualSorted(set)))
                    chainHeads.Add(set);
            }

            return new Hypergraph<T>(chainHeads);
        }

        public Hypergraph<T> Tr()
        {
            var H = new Hypergraph<T>(new List<List<T>>() { new List<T>() });

            foreach (var e in E)
                H = H.Product(EdgeHypergraph(e)).Min();

            return H;
        }

        Hypergraph<T> EdgeHypergraph(List<T> e)
        {
            return new Hypergraph<T>(e.Select(v => v.EnList()).ToList());
        }

        public Hypergraph<T> Union(Hypergraph<T> other)
        {
            return new Hypergraph<T>(E.Union(other.E).ToList());
        }

        public Hypergraph<T> Product(Hypergraph<T> other)
        {
            return new Hypergraph<T>(E.SelectMany(e_i => other.E.Select(e_j => ListUtility.Union(e_i, e_j)).ToList()).ToList());
        }

        class SortedListComparer : IEqualityComparer<List<T>>
        {
            public bool Equals(List<T> x, List<T> y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(List<T> list)
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
