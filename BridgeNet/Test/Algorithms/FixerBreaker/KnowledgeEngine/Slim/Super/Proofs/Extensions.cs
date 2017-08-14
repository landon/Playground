using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public static class Extensions
    {
        public static string GetArticle(this string letter)
        {
            switch (letter)
            {
                case "X":
                    return "an";
                case "Y":
                    return "a";
                case "Z":
                    return "a";
            }

            return "";
        }

        public static string Listify<T>(this IEnumerable<T> strings, string connector = "and")
        {
            var ll = strings.ToList();
            if (ll.Count <= 0)
                return "";
            if (ll.Count == 1)
                return ll[0].ToString();

            if (connector != null)
                return string.Join(", ", ll.Take(ll.Count - 1)) + " " + connector + " " + ll.Last();

            return string.Join(", ", ll);
        }

        public static string Wordify(this int n)
        {
            switch (n)
            {
                case 0:
                    return "first";
                case 1:
                    return "second";
                case 2:
                    return "third";
                case 3:
                    return "fourth";
                case 4:
                    return "fifth";
                case 5:
                    return "sixth";
                case 6:
                    return "seventh";
                case 7:
                    return "eighth";
                case 8:
                    return "ninth";
                case 9:
                    return "tenth";
                case 10:
                    return "eleventh";
            }

            return (n + 1) + "-th";
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T t)
        {
            return list.Except(new[] { t });
        }

        public static int MaxIndex<T>(this IEnumerable<T> list, Func<T, int> value)
        {
            var maxIndex = -1;
            var max = int.MinValue;
            int i = 0;
            foreach (var t in list)
            {
                var v = value(t);
                if (v > max)
                {
                    max = v;
                    maxIndex = i;
                }

                i++;
            }

            return maxIndex;
        }

        public static IEnumerable<T> Distinct<T, S>(this IEnumerable<T> list, Func<T, S> value)
        {
            var distinct = new List<T>();
            foreach (var t in list)
            {
                if (!distinct.Any(tt => value(tt).Equals(value(t))))
                    distinct.Add(t);
            }

            return distinct;
        }
    }
}

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs.ArbitraryMaxDegree
{
    public static class Extensions
    {
        public static string ToTex(this SequenceGeneralizer<int>.Matcher matcher, List<List<int>> lists, int listSize)
        {
            if (matcher.Name == "*")
                return string.Join("", Enumerable.Repeat("\\wild", listSize)) + " ";

            int i;
            if (!int.TryParse(matcher.Name, out i))
                return "?";

            return string.Join("", lists[i]);
        }

        public static int GetActiveListIndex(this int i, SuperSlimBoard b, int maxPot)
        {
            return b.Stacks.Value.Take(i + 1).Count(ss => ss.PopulationCount() < maxPot) - 1;
        }
    }
}

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs.MaxDegreeThree
{
    public static class Extensions
    {
        const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string ToXYZ(this SuperSlimBoard board)
        {
            return string.Join("", board.Stacks.Value.Select(ToXYZ));
        }

        public static List<int> To012(this SuperSlimBoard board)
        {
            return board.Stacks.Value.Select(To012).Where(x => x >= 0).ToList();
        }

        public static string ToCompactedPartitionId(this SuperSlimBoard board, List<List<int>> partition)
        {
            var xyz = board.Stacks.Value.Select(ToXYZ).ToList();
            return xyz.ToPartitionId(partition);
        }

        public static string ToPartitionId(this SuperSlimBoard board, List<List<int>> partition)
        {
            var xyz = board.Stacks.Value.Select(stack =>
                {
                    var s = stack.ToXYZ();
                    if (s.Length <= 0)
                        return "*";
                    return s;
                }).ToList();

            return xyz.ToPartitionId(partition);
        }

        public static string ToPartitionId(this List<string> xyz, List<List<int>> partition)
        {
            var pp = partition.OrderBy(part => part.Min()).ToList();
            for (int i = 0; i < pp.Count; i++)
            {
                var l = Alphabet[i].ToString();
                foreach (var j in pp[i])
                    xyz[j] = l;
            }

            return string.Join("", xyz);
        }

        public static string ToXYZ(this long stack)
        {
            switch (stack)
            {
                case 3:
                    return "X";
                case 5:
                    return "Y";
                case 6:
                    return "Z";
            }

            return "";
        }

        public static int To012(this long stack)
        {
            if (stack > 6)
                return -1;
            return ((int)stack / 2) - 1;
        }

        public static int GetXYZIndex(this int i, SuperSlimBoard b)
        {
            return b.Stacks.Value.Take(i + 1).Count(ss => ss.PopulationCount() == 2) - 1;
        }

        public static string ToTex(this SequenceGeneralizer<int>.Matcher matcher)
        {
            switch (matcher.Name)
            {
                case "*":
                    return "\\wild ";
                case "0":
                    return "X";
                case "1":
                    return "Y";
                case "2":
                    return "Z";
                case "!0":
                    return "\\bar{{X}}";
                case "!1":
                    return "\\bar{{Y}}";
                case "!2":
                    return "\\bar{{Z}}";
            }

            return "?";
        }
    }
}
