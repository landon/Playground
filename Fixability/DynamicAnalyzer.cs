using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public class DynamicAnalyzer<TColorSet, TVertexSet>
    {
        static Dictionary<int, List<List<List<int>>>> PartitionsCache = new Dictionary<int, List<List<List<int>>>>();

        IGraph<TColorSet, TVertexSet> _graph;
        IGraph<TColorSet, TVertexSet> _lineGraph;
        List<Tuple<int, int>> _edges;

        public void Initialize(IGraph<TColorSet, TVertexSet> graph, IGraph<TColorSet, TVertexSet> lineGraph, List<Tuple<int, int>> edges)
        {
            _graph = graph;
            _lineGraph = lineGraph;
            _edges = edges;
        }

        public bool Analyze(IAssignment<TColorSet, TVertexSet> assignment, HashSet<IAssignment<TColorSet, TVertexSet>> targets)
        {
            for (int i = 0; i < assignment.PotSize; i++)
            {
                for (int j = i + 1; j < assignment.PotSize; j++)
                {
                    var swappable = assignment.GetSwappable(i, j);

                    var winningSwapAlwaysExists = true;
                    foreach (var breakerChoice in GetBreakerChoices(swappable))
                    {
                        var winningSwapExists = false;

                        foreach (var response in GetFixerResponses(breakerChoice))
                        {
                            var swappedAssignment = assignment.PerformSwap(i, j, response);
                            if (targets.Contains(swappedAssignment))
                            {
                                winningSwapExists = true;
                                break;
                            }
                        }

                        if (!winningSwapExists)
                        {
                            winningSwapAlwaysExists = false;
                            break;
                        }
                    }

                    if (winningSwapAlwaysExists)
                        return true;
                }
            }

            return false;
        }

        List<TVertexSet> GetFixerResponses(List<TVertexSet> possibleMoves)
        {
            // this is weakly fixable, for fixable, union together any subset of the moves
            // TODO: implement the option to switch to full fixable

            return possibleMoves;
        }

        List<List<TVertexSet>> GetBreakerChoices(TVertexSet swappable)
        {
            var allPairingParitions = GetPairingPartitions(swappable);
            
            // TODO: remove illegal choices for a given class of graphs

            return allPairingParitions;
        }

        List<List<TVertexSet>> GetPairingPartitions(TVertexSet swappable)
        {
            var count = _graph.Operations.Count(swappable);
            return GetPartitions(count).Select(p => p.Select(pp => _graph.Operations.Subset(swappable, pp)).ToList())
                                       .ToList();
        }

        static List<List<List<int>>> GetPartitions(int count)
        {
            List<List<List<int>>> partitions;
            if (!PartitionsCache.TryGetValue(count, out partitions))
            {
                partitions = EnumeratePartitions(count).ToList();
                PartitionsCache[count] = partitions;
            }

            return partitions;
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
    }
}
