using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Utility
{
    public static class LinqExtensions
    {
        public static IEnumerable<Tuple<T, T>> CartesianProduct<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            return CartesianProduct(new[] { a, b }).Select(s => new Tuple<T, T>(s.ElementAt(0), s.ElementAt(1)));
        }
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<List<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> e, T value)
        {
            foreach (var v in e)
                yield return v;
            yield return value;
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> areEqual)
        {
            var yielded = new List<T>();

            foreach (var t in source)
            {
                if (yielded.All(x => !areEqual(x, t)))
                {
                    yielded.Add(t);
                    yield return t;
                }
            }
        }
    }
}
