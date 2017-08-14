using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.Chronicle
{
    public static class BranchGenerator
    {
        static Dictionary<int, List<List<List<int>>>> PartitionsCache = new Dictionary<int, List<List<List<int>>>>();

        public static List<List<List<int>>> GetPartitions(int count)
        {
            List<List<List<int>>> partitions;
            if (!PartitionsCache.TryGetValue(count, out partitions))
            {
                partitions = EnumeratePartitions(count).ToList();
                PartitionsCache[count] = partitions;
            }

            return partitions;
        }
        public static IEnumerable<ChronicledBranch> EnumerateBranches(Board board, Tuple<int, int> colorPair)
        {
            var S = EnumerateExactlyOneIntersecters(board, colorPair).ToList();
            var partitions = GetPartitions(S.Count);

            return partitions.Select(partition => new ChronicledBranch(S, colorPair.Item1, colorPair.Item2, partition));
        }

        static IEnumerable<List<List<int>>> EnumeratePartitions(int n)
        {
            if (n % 2 == 0)
            {
                foreach (var partition in EnumeratePartitions(Enumerable.Range(0, n).ToList()))
                    yield return partition;
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    foreach (var partition in EnumeratePartitions(Enumerable.Range(0, n).Where(j => j != i).ToList()))
                    {
                        partition.Insert(0, new List<int>() { i });
                        yield return partition;
                    }
                }
            }
        }

        static IEnumerable<List<List<int>>> EnumeratePartitions(List<int> indices)
        {
            if (indices.Count <= 0)
                yield return new List<List<int>>();
            else
            {
                var first = indices[0];
                indices.RemoveAt(0);
                for (int i = 0; i < indices.Count; i++)
                {
                    var part = new List<int> { first, indices[i] };
                    indices.RemoveAt(i);

                    foreach (var partition in EnumeratePartitions(indices))
                    {
                        partition.Add(part);
                        yield return partition;
                    }

                    indices.Insert(i, part[1]);
                }

                indices.Insert(0, first);
            }
        }

        public static IEnumerable<int> EnumerateExactlyOneIntersecters(Board board, Tuple<int, int> colorPair)
        {
            return Enumerable.Range(0, board.Stacks.Count).Where(i => board.Stacks[i].IsBitSet(colorPair.Item1) ^ board.Stacks[i].IsBitSet(colorPair.Item2));
        }
    }
}
