using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;

namespace Algorithms.WordGame
{
    public static class Boxification
    {
        public static List<Tuple<List<List<T>>, List<List<T>>>> Split<T>(List<List<T>> allPoints)
        {
            var chunks = new List<Tuple<List<List<T>>, List<List<T>>>>();

            var points = allPoints.ToList();
            int n = allPoints[0].Count;

            while (points.Count > 0)
            {
                int bestT = 1;
                int bestSplit = 1;

                for (int split = 1; split < n - 1; split++)
                {
                    var left = points.Select(p => p.Take(split).ToList()).ToList();
                    var right = points.Select(p => p.Skip(split).ToList()).ToList();

                    var t = Binary(1, points.Count - 1, i =>
                    {
                        var lefts = left.Take(i).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList();
                        var rights = right.Take(i).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList();

                        var span = lefts.CartesianProduct(rights).Select(tup => tup.Item1.Concat(tup.Item2).ToList()).ToList();
                        var allExist = span.All(p => points.Any(pp => pp.SequenceEqual(p)));

                        return allExist ? 1 : -1;
                    }) - 1;

                    if (t >= bestT)
                    {
                        if (t > bestT || Math.Abs(n/2 - split) < Math.Abs(n/2 - bestSplit))
                        {
                            bestT = t;
                            bestSplit = split;
                        }
                    }
                }

                var l = points.Select(p => p.Take(bestSplit).ToList()).ToList();
                var r = points.Select(p => p.Skip(bestSplit).ToList()).ToList();
                var ls = l.Take(bestT).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList();
                var rs = r.Take(bestT).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList();
                var s = ls.CartesianProduct(rs).Select(tup => tup.Item1.Concat(tup.Item2).ToList()).ToList();

                chunks.Add(new Tuple<List<List<T>>, List<List<T>>>(ls, rs));
                points.RemoveAll(p => s.Any(pp => pp.SequenceEqual(p)), (x) => { });
            }

            return chunks;
        }

        public static List<List<List<List<T>>>> SplitMultiway<T>(List<List<T>> allPoints)
        {
            var chunks = new List<List<List<List<T>>>>();

            var points = allPoints.ToList();
            int n = allPoints[0].Count;

            while (points.Count > 0)
            {
                int bestT = 1;
                List<int> bestSplit = new List<int>();

                foreach (var split in Enumerable.Range(0, n).ToList().EnumerateSublists().Select(l => l.OrderBy(x => x).ToList()))
                {
                    if (ExtractRanges(split, points[0]).Count <= 1)
                        continue;

                    var t = Binary(1, points.Count - 1, i =>
                    {
                        var ranges = points.Take(i).Select(p => ExtractRanges(split, p)).ToList();
                        var rangeValues = ranges.Aggregate(Enumerable.Range(0, ranges[0].Count).Select(__ => new List<List<T>>()).ToList(),
                            (union, next) => union.Zip(next, (A, B) => A.Concat(B).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList()).ToList());

                        var cp = rangeValues.CartesianProduct().Select(ll => ll.ToList()).ToList();
                        var span = cp.Select(ll => ll.SelectMany(pp => pp).ToList()).ToList();
                        var allExist = span.All(p => points.Any(pp => pp.SequenceEqual(p)));

                        return allExist ? 1 : -1;
                    }) - 1;

                    if (t > bestT)
                    {
                        bestT = t;
                        bestSplit = split;
                    }
                }

                var rr = points.Take(bestT).Select(p => ExtractRanges(bestSplit, p)).ToList();
                var rrv = rr.Aggregate(Enumerable.Range(0, rr[0].Count).Select(__ => new List<List<T>>()).ToList(),
                    (union, next) => union.Zip(next, (A, B) => A.Concat(B).Distinct((l1, l2) => l1.SequenceEqual(l2)).ToList()).ToList());

                var s = rrv.CartesianProduct().Select(ll => ll.SelectMany(pp => pp).ToList()).ToList();

                chunks.Add(rrv);
                points.RemoveAll(p => s.Any(pp => pp.SequenceEqual(p)), (x) => { });
            }

            return chunks;
        }

        static List<List<T>> ExtractRanges<T>(List<int> stops, List<T> l)
        {
            var ranges = new List<List<T>>();

            var left = 0;
            foreach (var stop in stops)
            {
                ranges.Add(l.GetRange(left, stop - left + 1));
                left = stop + 1;
            }

            if (left < l.Count)
                ranges.Add(l.GetRange(left, l.Count - left));

            return ranges;
        }

        public static List<List<List<T>>> Boxify<T>(List<List<T>> points)
        {
            var boxes = points.Select(p => p.EnList()).ToList();
            while (true)
            {
                var boxContainers = boxes.CartesianProduct(boxes).Where(pair => !pair.Item1.SequenceEqual(pair.Item2)).Select(pair => new { Pair = pair, BoxedUnion = BoxContainer(pair.Item1.Concat(pair.Item2).ToList()) });

                var grown = boxContainers.FirstOrDefault(bc => ContainsBoxContainer(points, bc.BoxedUnion));
                if (grown == null)
                    break;

                var count = boxes.Count;
                boxes.Remove(grown.Pair.Item1);
                boxes.Remove(grown.Pair.Item2);
                boxes.Add(grown.BoxedUnion);

                if (boxes.Count == count)
                    break;
            }

            return boxes;
        }

        public static List<List<List<T>>> PrefixBoxify<T>(List<List<T>> points)
        {
            var allPoints = points.ToList();
            var boxes = new List<List<List<T>>>();

            while (true)
            {
                var t = Binary(1, points.Count - 1, i =>
                    {
                        return ContainsBox(allPoints, points.Take(i).ToList()) ? 1 : -1;
                    });

                boxes.Add(BoxContainer(points.Take(t).ToList()));
                points = points.Skip(t).ToList();
                if (points.Count <= 0)
                    break;
            }

            return boxes;
        }

        public static bool ContainsBox<T>(List<List<T>> allPoints, List<List<T>> points)
        {
            var box = BoxContainer(points);
            return ContainsBoxContainer(allPoints, box);
        }

        static bool ContainsBoxContainer<T>(List<List<T>> allPoints, List<List<T>> boxContainer)
        {
            return boxContainer.All(p => allPoints.Any(pp => pp.SequenceEqual(p)));
        }

        public static List<List<T>> BoxContainer<T>(List<List<T>> points)
        {
            return Enumerable.Range(0, points[0].Count).Select(i => points.Select(p => p[i]).Distinct().ToList())
                                                       .CartesianProduct()
                                                       .Select(e => e.ToList())
                                                       .ToList();
        }

        public static bool IsBox<T>(List<List<T>> points)
        {
            return points.Count == BoxContainer(points).Count;
        }

        public static string ToBoxString<T>(List<List<T>> points)
        {
            return string.Join(" * ", Enumerable.Range(0, points[0].Count).Select(i => "{" + string.Join(",", points.Select(p => p[i]).Distinct()) + "}"));
        }

        public static string ToMultiChunkString<T>(List<List<List<T>>> multiChunk)
        {
            return string.Join(" * ", multiChunk.Select(chunk => ToListOfListsString(chunk)));
        }

        public static string ToChunkString<T>(Tuple<List<List<T>>, List<List<T>>> chunk)
        {
            return ToListOfListsString(chunk.Item1) + " * " + ToListOfListsString(chunk.Item2);
        }

        static string ToListOfListsString<T>(List<List<T>> l)
        {
            return "{" + string.Join(",", l.Select(sl => "{" + string.Join(",", sl) + "}")) + "}";
        }

        static int Binary(int first, int last, Func<int, int> targetDirection)
        {
            var left = first - 1;
            var right = last + 1;

            while (right - left >= 2)
            {
                int middle = left + (right - left) / 2;
                int direction = targetDirection(middle);

                if (direction == 0)
                    return middle;
                else if (direction < 0)
                    right = middle;
                else
                    left = middle;
            }

            if (left < first)
                return first - 1;
            if (right > last)
                return last + 1;

            return left;
        }
    }
}
