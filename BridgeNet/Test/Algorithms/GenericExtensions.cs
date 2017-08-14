using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class GenericExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void ShuffleAll(this IList<IList> lists)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            int n = lists[0].Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);

                foreach (var list in lists)
                {
                    var value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }

        public static List<T> EnList<T>(this T t)
        {
            return new List<T>() { t };
        }

        public static IEnumerable<int> IndicesWhere<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            var i = 0;
            foreach(var t in source)
            {
                if (condition(t))
                    yield return i;

                i++;
            }
        }

        public static int FirstIndex<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            var i = 0;
            foreach (var t in source)
            {
                if (condition(t))
                    return i;

                i++;
            }

            return -1;
        }

        public static string JoinPretty(this IEnumerable<string> source, string separator)
        {
            var count = source.Count();
            if (count <= 0)
                return "";
            if (count == 1)
                return source.First();
            if (count == 2)
                return source.ElementAt(0) + " and " + source.ElementAt(1);

            return string.Join(separator, source.Take(count - 1)) + " and " + source.ElementAt(count - 1);
        }

        public static int RemoveAll<T>(this List<T> list, Func<T, bool> predicate, Action<T> onRemoved)
        {
            var removed = 0;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    onRemoved(list[i]);
                 
                    list.RemoveAt(i);
                    removed++;
                }
            }

            return removed;
        }
    }
}
