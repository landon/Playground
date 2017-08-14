using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace Algorithms.Utility
{
    public static class ListUtility
    {
        static class ListUtilityGeneric<T>
        {
            static HashSet<T> _set = new HashSet<T>();
            static Dictionary<T, int> _multiSet = new Dictionary<T, int>();

            public static List<T> Intersection(IEnumerable<T> A, IEnumerable<T> B)
            {
                if (A == null || B == null)
                    return new List<T>();

                var inB = _set;
                inB.Clear();

                foreach (var t in B)
                    inB.Add(t);

                var intersection = new List<T>(inB.Count);
                foreach (var t in A)
                {
                    if (inB.Contains(t))
                        intersection.Add(t);
                }

                return intersection;
            }

            public static int IntersectionCount(IEnumerable<T> A, IEnumerable<T> B)
            {
                if (A == null || B == null)
                    return 0;

                var inB = _set;
                inB.Clear();

                foreach (var t in B)
                    inB.Add(t);

                var count = 0;
                foreach (var t in A)
                {
                    if (inB.Contains(t))
                        count++;
                }

                return count;
            }

            public static List<T> Difference(IEnumerable<T> A, IEnumerable<T> B)
            {
                if (A == null)
                    return new List<T>();
                if (B == null)
                    B = new List<T>();

                var inB = _set;
                inB.Clear();

                foreach (var t in B)
                    inB.Add(t);

                var difference = new List<T>(inB.Count);
                foreach (var t in A)
                {
                    if (!inB.Contains(t))
                        difference.Add(t);
                }

                return difference;
            }

            public static List<T> Union(List<T> A, List<T> B)
            {
                if (A == null && B == null)
                    return new List<T>();
                if (A == null)
                    return B;
                if (B == null)
                    return A;

                var inB = _set;
                inB.Clear();

                foreach (T t in B)
                    inB.Add(t);

                var union = new List<T>(B);
                foreach (T t in A)
                {
                    if (!inB.Contains(t))
                        union.Add(t);
                }

                return union;
            }

            public static List<T> Union(List<T> A, T b)
            {
                var union = new List<T>(A);
                if (!union.Contains(b))
                    union.Add(b);

                return union;
            }

            public static List<T> MultiSetIntersection(IEnumerable<T> A, IEnumerable<T> B)
            {
                if (A == null || B == null)
                    return new List<T>();

                var inB = _multiSet;
                inB.Clear();

                foreach (var t in B)
                {
                    int count;
                    inB.TryGetValue(t, out count);

                    inB[t] = count + 1;
                }

                var intersection = new List<T>(inB.Count);
                foreach (var t in A)
                {
                    int count;
                    inB.TryGetValue(t, out count);

                    if (count > 0)
                    {
                        intersection.Add(t);
                        inB[t] = count - 1;
                    }
                }

                return intersection;
            }

            public static int MultiSetIntersectionCount(IEnumerable<T> A, IEnumerable<T> B)
            {
                if (A == null || B == null)
                    return 0;

                var inB = _multiSet;
                inB.Clear();

                foreach (var t in B)
                {
                    int count;
                    inB.TryGetValue(t, out count);

                    inB[t] = count + 1;
                }

                var total = 0;
                foreach (var t in A)
                {
                    int count;
                    inB.TryGetValue(t, out count);

                    if (count > 0)
                    {
                        total++;
                        inB[t] = count - 1;
                    }
                }

                return total;
            }
        }

        #region sorted lists
        public static List<T> DifferenceSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            if (B == null)
                return A.ToList();

            var C = new List<T>(Math.Max(4, A.Count - B.Count));
            int j = 0;
            for (int i = 0; i < A.Count; i++)
            {
                while (j < B.Count && A[i].CompareTo(B[j]) > 0)
                    j++;

                if (j >= B.Count || A[i].CompareTo(B[j]) < 0)
                    C.Add(A[i]);
            }

            return C;
        }

        public static List<T> IntersectionSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            if (A == null || B == null)
                return new List<T>();

            var C = new List<T>(Math.Min(A.Count, B.Count));
            int i = 0;
            int j = 0;

            while (i < A.Count && j < B.Count)
            {
                var compare = A[i].CompareTo(B[j]);

                if (compare == 0)
                    C.Add(A[i]);
                if (compare <= 0)
                    i++;
                if (compare >= 0)
                    j++;
            }

            return C;
        }

        public static int IntersectionCountSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            if (A == null || B == null)
                return 0;

            var count = 0;
            int i = 0;
            int j = 0;

            while (i < A.Count && j < B.Count)
            {
                var compare = A[i].CompareTo(B[j]);

                if (compare == 0)
                    count++;
                if (compare <= 0)
                    i++;
                if (compare >= 0)
                    j++;
            }

            return count;
        }

        public static List<T> UnionSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            var list = A.DifferenceSorted(B);
            list.AddRange(B.DifferenceSorted(A));

            return list;
        }

        public static bool EqualSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            return A.Count == B.Count && IntersectionCountSorted(A, B) == A.Count;
        }

        public static bool SubsetEqualSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            return IntersectionCountSorted(A, B) == A.Count;
        }

        public static bool SubsetSorted<T>(this List<T> A, List<T> B)
            where T : IComparable
        {
            return A.Count < B.Count && IntersectionCountSorted(A, B) == A.Count;
        }

        public static List<List<T>> MaximalElementsSorted<T>(this List<List<T>> A)
            where T : IComparable
        {
            if (A == null || A.Count <= 0)
                return new List<List<T>>();

            var chainHeads = new List<List<T>>();

            foreach (var set in A)
            {
                var removed = chainHeads.RemoveAll(head => head.SubsetEqualSorted(set), (x) => { } );
                if (removed > 0 || chainHeads.All(head => !set.SubsetEqualSorted(head)))
                    chainHeads.Add(set);
            }

            return chainHeads;
        }

        #region int specific
        public static List<int> DifferenceSorted(this List<int> A, List<int> B)
        {
            if (B == null)
                return A.ToList();

            var C = new List<int>(Math.Max(4, A.Count - B.Count));
            int j = 0;
            for (int i = 0; i < A.Count; i++)
            {
                while (j < B.Count && A[i] > B[j])
                    j++;

                if (j >= B.Count || A[i] < B[j])
                    C.Add(A[i]);
            }

            return C;
        }

        public static List<int> IntersectionSorted(this List<int> A, List<int> B)
        {
            if (A == null || B == null)
                return new List<int>();

            var C = new List<int>(Math.Min(A.Count, B.Count));
            int i = 0;
            int j = 0;

            int ac = A.Count;
            int bc = B.Count;
            while (i < ac && j < bc)
            {
                var a = A[i];
                var b = B[j];

                if (a == b)
                    C.Add(a);
                if (a <= b)
                    i++;
                if (a >= b)
                    j++;
            }

            return C;
        }

        public static int IntersectionCountSorted(this List<int> A, List<int> B)
        {
            if (A == null || B == null)
                return 0;

            var count = 0;
            int i = 0;
            int j = 0;

            int ac = A.Count;
            int bc = B.Count;
            while (i < ac && j < bc)
            {
                var a = A[i];
                var b = B[j];

                if (a == b)
                    count++;
                if (a <= b)
                    i++;
                if (a >= b)
                    j++;
            }

            return count;
        }

        public static List<int> UnionSorted(this List<int> A, List<int> B)
        {
            var list = A.DifferenceSorted(B);
            list.AddRange(B.DifferenceSorted(A));

            return list;
        }

        public static bool EqualSorted(this List<int> A, List<int> B)
        {
            return A.Count == B.Count && IntersectionCountSorted(A, B) == A.Count;
        }

        public static bool SubsetEqualSorted(this List<int> A, List<int> B)
        {
            return IntersectionCountSorted(A, B) == A.Count;
        }

        public static bool SubsetSorted(this List<int> A, List<int> B)
        {
            return A.Count < B.Count && IntersectionCountSorted(A, B) == A.Count;
        }

        public static List<List<int>> MaximalElementsSorted(this List<List<int>> A)
        {
            if (A == null || A.Count <= 0)
                return new List<List<int>>();

            var chainHeads = new List<List<int>>();

            foreach (var set in A)
            {
                var removed = chainHeads.RemoveAll(head => head.SubsetEqualSorted(set), (x) => { });
                if (removed > 0 || chainHeads.All(head => !set.SubsetEqualSorted(head)))
                    chainHeads.Add(set);
            }

            return chainHeads;
        }
        #endregion
        #endregion

        public static List<T> MultiSetIntersection<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            return ListUtilityGeneric<T>.MultiSetIntersection(A, B);
        }

        public static int MultiSetIntersectionCount<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            return ListUtilityGeneric<T>.MultiSetIntersectionCount(A, B);
        }

        public static bool MultiSetEqual<T>(this List<T> A, List<T> B)
        {
            return A.Count == B.Count && MultiSetIntersectionCount(A, B) == A.Count;
        }

        public static bool MultiSetSubsetEqual<T>(this List<T> A, List<T> B)
        {
            return MultiSetIntersectionCount(A, B) == A.Count;
        }

        public static bool MultiSetSubset<T>(this List<T> A, List<T> B)
        {
            return A.Count < B.Count && MultiSetIntersectionCount(A, B) == A.Count;
        }

        public static long BinomialCoefficient(long n, long k)
        {
            if (k < 0)
            {
                return 1;
            }

            long max = Math.Max(k, n - k);
            long min = Math.Min(k, n - k);

            long result = 1;
            while (n > max)
            {
                result *= n;
                n--;
            }

            while (min > 0)
            {
                result /= min;

                min--;
            }

            return result;
        }

        public static string ToString<T>(List<T> A)
        {
            var sb = new StringBuilder();

            foreach (T t in A)
                sb.AppendFormat("{0}, ", t.ToString());

            return sb.ToString().TrimEnd(' ', ',');
        }

        public static List<T> Difference<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            return ListUtilityGeneric<T>.Difference(A, B);
        }

        public static List<T> Intersection<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            return ListUtilityGeneric<T>.Intersection(A, B);
        }

        public static int IntersectionCount<T>(this IEnumerable<T> A, IEnumerable<T> B)
        {
            return ListUtilityGeneric<T>.IntersectionCount(A, B);
        }

        public static List<T> Union<T>(this List<T> A, List<T> B)
        {
            return ListUtilityGeneric<T>.Union(A, B);
        }

        public static List<T> Union<T>(this List<T> A, T b)
        {
            return ListUtilityGeneric<T>.Union(A, b);
        }

        public static bool Equal<T>(this List<T> A, List<T> B)
        {
            return A.Count == B.Count && IntersectionCount(A, B) == A.Count;
        }

        public static bool SubsetEqual<T>(this List<T> A, List<T> B)
        {
            return IntersectionCount(A, B) == A.Count;
        }

        public static bool Subset<T>(this List<T> A, List<T> B)
        {
            return A.Count < B.Count && IntersectionCount(A, B) == A.Count;
        }

        public static List<List<T>> MaximalElements<T>(this List<List<T>> A)
        {
            if (A == null || A.Count <= 0)
                return new List<List<T>>();

            var chainHeads = new List<List<T>>();

            foreach (var set in A)
            {
                var removed = chainHeads.RemoveAll(head => head.SubsetEqual(set), (x) => { });
                if (removed > 0 || chainHeads.All(head => !set.SubsetEqual(head)))
                    chainHeads.Add(set);
            }

            return chainHeads;
        }

        public static List<List<T>> GenerateSublists<T>(this List<T> list)
        {
            List<List<T>> sublists = new List<List<T>>();
            sublists.Add(new List<T>());

            List<T> clone = new List<T>(list);

            while (clone.Count > 0)
            {
                T head = clone[0];

                int count = sublists.Count;
                for (int i = 0; i < count; i++)
                {
                    List<T> union = new List<T>(sublists[i]);
                    union.Add(head);

                    sublists.Add(union);
                }

                clone.RemoveAt(0);
            }

            return sublists;
        }

        public static List<List<T>> GenerateSublists<T>(this List<T> list, int size)
        {
            List<List<T>> sublists = new List<List<T>>();
            int n = list.Count;
            foreach (List<bool> inOut in EnumerateShortLex(n, size))
                sublists.Add(ExtractSublist(list, inOut));

            return sublists;
        }

        public static IEnumerable<List<T>> EnumerateSublists<T>(this List<T> list)
        {
            int n = list.Count;
            foreach (List<bool> inOut in EnumerateShortLex(n))
                yield return ExtractSublist(list, inOut);
        }

        public static IEnumerable<List<T>> EnumerateSublists<T>(this List<T> list, int size)
        {
            int n = list.Count;
            foreach (List<bool> inOut in EnumerateShortLex(n, size))
                yield return ExtractSublist(list, inOut);
        }

        public static List<T> NextSublist<T>(List<T> list, int size, ref List<bool> state)
        {
            int n = list.Count;
            bool areMore = NextShortLex(n, size, ref state);

            List<T> sublist = ExtractSublist(list, state);

            if (!areMore)
            {
                state = null;
            }

            return sublist;
        }

        static List<T> ExtractSublist<T>(List<T> list, List<bool> inOut)
        {
            List<T> sublist = new List<T>();
            for (int i = 0; i < inOut.Count; i++)
            {
                if (inOut[i])
                    sublist.Add(list[i]);
            }

            return sublist;
        }

        static IEnumerable<List<bool>> EnumerateShortLex(int n, int k)
        {
            k = Math.Max(0, k);

            List<bool> list = new List<bool>(n);
            for (int i = 0; i < k; i++)
                list.Add(true);
            for (int i = k; i < n; i++)
                list.Add(false);

            do
            {
                yield return list;
            }
            while (NextSameTrueCount(list, k));
        }

        public static IEnumerable<List<bool>> EnumerateShortLex(int n)
        {
            var list = new List<bool>(n);
            for (int i = 0; i < n; i++)
                list.Add(false);

            do
            {
                yield return list;
            }
            while (Next(list));
        }

        public static IEnumerable<List<bool>> EnumerateShortLexNonempty(int n)
        {
            for (int k = 1; k <= n; k++)
                foreach (var list in EnumerateShortLex(n, k))
                    yield return list;
        }

        static bool NextShortLex(int n, int k, ref List<bool> state)
        {
            if (state == null)
            {
                k = Math.Max(0, k);

                state = new List<bool>(n);
                for (int i = 0; i < k; i++)
                    state.Add(true);
                for (int i = k; i < n; i++)
                    state.Add(false);

                return k > 0 && k < n;
            }

            return NextSameTrueCount(state, k);
        }

        static bool Next(List<bool> list)
        {
            int trueCount = ((IEnumerable<bool>)list).Count(b => b);
            if (trueCount == list.Count)
                return false;

            int firstHole = FindFirstHole(list);

            if (firstHole >= list.Count)
            {
                for (int i = 0; i < trueCount + 1; i++)
                    list[i] = true;
                for (int i = trueCount + 1; i < list.Count; i++)
                    list[i] = false;
            }
            else
            {
                list[firstHole] = true;

                int initialTrues = trueCount;
                for (int i = firstHole; i < list.Count; i++)
                {
                    if (list[i])
                        initialTrues--;
                }

                for (int i = 0; i < initialTrues; i++)
                    list[i] = true;
                for (int i = initialTrues; i < firstHole; i++)
                    list[i] = false;
            }

            return true;
        }

        static bool NextSameTrueCount(List<bool> list, int trueCount)
        {
            int firstHole = FindFirstHole(list);

            if (firstHole >= list.Count)
                return false;

            list[firstHole] = true;

            int initialTrues = trueCount;
            for (int i = firstHole; i < list.Count; i++)
            {
                if (list[i])
                    initialTrues--;
            }

            for (int i = 0; i < initialTrues; i++)
                list[i] = true;
            for (int i = initialTrues; i < firstHole; i++)
                list[i] = false;

            return true;
        }

        static int FindFirstHole(List<bool> list)
        {
            int i = 0;
            while (i < list.Count - 1)
            {
                if (list[i] && !list[i + 1])
                    break;

                i++;
            }

            return i + 1;
        }

        #region int based
        public static bool NextShortLex(int n, int k, ref int[] state)
        {
            if (state == null)
            {
                k = Math.Max(0, k);

                state = new int[n];
                for (int i = 0; i < k; i++)
                    state[i] = 1;

                return k <= 0 || k >= n;
            }

            bool done = !NextSameOnCount(state, k);

            if (done)
            {
                for (int i = 0; i < k; i++)
                    state[i] = 1;

                for (int i = k; i < n; i++)
                    state[i] = 0;
            }

            return done;
        }

        static bool NextSameOnCount(int[] list, int trueCount)
        {
            int firstHole = FindFirstHole(list);

            if (firstHole >= list.Length)
                return false;

            list[firstHole] = 1;

            int initialOnes = trueCount;
            for (int i = firstHole; i < list.Length; i++)
            {
                initialOnes -= list[i];
            }

            for (int i = 0; i < initialOnes; i++)
                list[i] = 1;
            for (int i = initialOnes; i < firstHole; i++)
                list[i] = 0;

            return true;
        }

        static int FindFirstHole(int[] list)
        {
            int i = 0;
            while (i < list.Length - 1)
            {
                if (list[i] == 1 && list[i + 1] == 0)
                    break;

                i++;
            }

            return i + 1;
        }

        #endregion

        #region partitions
        public static IEnumerable<List<List<int>>> EnumeratePartitionsUnordered(int[] sizes)
        {
            return EnumeratePartitions(sizes, 0).Distinct((aa, bb) => PartialOrder.Embeds(aa, bb, (a, b) => a.Equal(b) ? new PartialOrderResult() { AtMost = true, AtLeast = true } : new PartialOrderResult()));
        }

        public static IEnumerable<List<List<int>>> EnumeratePartitions(int[] sizes)
        {
            return EnumeratePartitions(sizes, 0);
        }

        static IEnumerable<List<List<int>>> EnumeratePartitions(int[] sizes, int item)
        {
            var childless = true;
            for (int i = 0; i < sizes.Length; i++)
            {
                if (sizes[i] <= 0)
                    continue;

                childless = false;

                sizes[i]--;

                foreach (var partition in EnumeratePartitions(sizes, item + 1))
                {
                    partition[i].Add(item);
                    yield return partition;
                }

                sizes[i]++;
            }

            if (childless)
            {
                var partition = new List<List<int>>();
                for (int i = 0; i < sizes.Length; i++)
                    partition.Add(new List<int>());

                yield return partition;
            }
        }
        #endregion

    }
}