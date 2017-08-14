using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Algorithms.Utility;
using Algorithms.FixerBreaker;

namespace Algorithms
{
    public class Graph
    {
        static Random RNG = new Random(DateTime.Now.Millisecond);
        public static long CacheHits;
        public static long NodesVisited;

        bool[,] _adjacent;
        Lazy<List<List<int>>> _neighbors;
        Lazy<List<List<int>>> _complementNeighbors;
        Lazy<List<List<int>>> _outNeighbors;
        Lazy<List<List<int>>> _inNeighbors;
        public Lazy<List<List<int>>> _laterNeighbors;
        List<int> _vertices;
        Lazy<List<List<int>>> _independentSets;
        Lazy<List<List<int>>> _maximalIndependentSets;
        Lazy<List<List<int>>> _independentTwoSets;
        Lazy<List<List<int>>> _independentThreeSets;
        Lazy<List<List<int>>> _vertexSubsets;
        Lazy<int> _e;

        public int N { get; private set; }
        public int E { get { return _e.Value; } }

        public bool[,] Adjacent { get { return _adjacent; } }
        public bool this[int x, int y]
        {
            get { return _adjacent[x, y]; }
        }

        public bool[,] Directed
        {
            get;
            private set;
        }

        public List<int> VertexWeight { get; set; }
        public List<List<int>> Neighbors { get { return _neighbors.Value; } }
        public List<List<int>> ComplementNeighbors { get { return _complementNeighbors.Value; } }
        public List<List<int>> OutNeighbors { get { return _outNeighbors.Value; } }
        public List<List<int>> InNeighbors { get { return _inNeighbors.Value; } }
        public List<int> Vertices { get { return _vertices; } }
        public Lazy<int[]> DegreeSequence { get; private set; }
        public Lazy<int[]> InDegreeSequence { get; private set; }
        public Lazy<int[]> OutDegreeSequence { get; private set; }
        public Lazy<int[]> NodeInvariantOne { get; private set; }
        public Lazy<int[]> NodeInvariantTwo { get; private set; }
        TransitivePartition TransitivePartition { get; set; }
        public List<int> EdgeWeightsWithMultiplicity { get; set; }

        public List<List<int>> IndependentSets { get { return _independentSets.Value; } }
        public List<List<int>> MaximalIndependentSets { get { return _maximalIndependentSets.Value; } }

        public Graph FromOutNeighborLists(Dictionary<int, List<int>> outNeighbors)
        {
            var N = outNeighbors.Count;

            var edgeWeights = new List<int>();
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (outNeighbors[i].Contains(j))
                        edgeWeights.Add(1);
                    else if (outNeighbors[j].Contains(i))
                        edgeWeights.Add(-1);
                    else
                        edgeWeights.Add(0);
                }
            }

            return new Graph(edgeWeights);
        }

        public Graph(List<int> edgeWeights, List<int> vertexWeight = null)
        {
            N = (int)((1 + Math.Sqrt(1 + 8 * edgeWeights.Count)) / 2);
            VertexWeight = vertexWeight;

            _vertices = Enumerable.Range(0, N).ToList();
            _adjacent = new bool[N, N];
            Directed = new bool[N, N];

            int k = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (edgeWeights[k] != 0)
                    {
                        _adjacent[i, j] = true;
                        _adjacent[j, i] = true;

                        if (edgeWeights[k] > 0)
                            Directed[i, j] = true;
                        if (edgeWeights[k] < 0)
                            Directed[j, i] = true;
                    }

                    k++;
                }
            }

            Initialize();
        }
        public Graph(bool[,] adjacent, List<int> vertexWeight = null)
        {
            _adjacent = adjacent;
            N = _adjacent.GetUpperBound(0) + 1;
            VertexWeight = vertexWeight;

            _vertices = Enumerable.Range(0, N).ToList();
            Directed = new bool[N, N];

            Initialize();
        }

        void Initialize()
        {
            _e = new Lazy<int>(() =>
                {
                    int e = 0;

                    for (int i = 0; i < N; i++)
                    {
                        for (int j = i + 1; j < N; j++)
                        {
                            if (_adjacent[i, j])
                                e++;
                        }
                    }

                    return e;
                });

            SetupNeighbors();
            InitializeLazyLoaders();
        }

        void SetupNeighbors()
        {
            _neighbors = new Lazy<List<List<int>>>(() =>
                {
                    var neighbors = new List<List<int>>();
                    for (int i = 0; i < N; i++)
                        neighbors.Add(new List<int>());

                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                            if (_adjacent[i, j])
                                neighbors[i].Add(j);

                    return neighbors;
                });

            _complementNeighbors = new Lazy<List<List<int>>>(() =>
            {
                var nn = new List<List<int>>();
                for (int i = 0; i < N; i++)
                    nn.Add(new List<int>());

                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (i != j && !_adjacent[i, j])
                            nn[i].Add(j);

                return nn;
            });

            _outNeighbors = new Lazy<List<List<int>>>(() =>
            {
                var neighbors = new List<List<int>>();
                for (int i = 0; i < N; i++)
                    neighbors.Add(new List<int>());

                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (_adjacent[i, j] && Directed[i, j])
                            neighbors[i].Add(j);

                return neighbors;
            });

            _inNeighbors = new Lazy<List<List<int>>>(() =>
            {
                var neighbors = new List<List<int>>();
                for (int i = 0; i < N; i++)
                    neighbors.Add(new List<int>());

                for (int i = 0; i < N; i++)
                    for (int j = 0; j < N; j++)
                        if (_adjacent[i, j] && Directed[i, j])
                            neighbors[j].Add(i);

                return neighbors;
            });

            _laterNeighbors = new Lazy<List<List<int>>>(() =>
                {
                    var later = new List<List<int>>(Neighbors.Count);

                    for(int i = 0; i < N; i++)
                        later.Add(Neighbors[i].Where(n => n > i).ToList());

                    return later;
                });

            TransitivePartition = new TransitivePartition(_vertices);

            DegreeSequence = new Lazy<int[]>(() =>
                {
                    var s = new int[N];
                    foreach (var v in _vertices)
                        s[v] = Degree(v);

                    return s;
                });

            InDegreeSequence = new Lazy<int[]>(() =>
            {
                var s = new int[N];
                foreach (var v in _vertices)
                    s[v] = InDegree(v);

                return s;
            });

            OutDegreeSequence = new Lazy<int[]>(() =>
            {
                var s = new int[N];
                foreach (var v in _vertices)
                    s[v] = OutDegree(v);

                return s;
            });

            NodeInvariantOne = new Lazy<int[]>(() =>
                {
                    var s = new int[N];
                    var sp = new int[N];
                    foreach (var v in _vertices)
                        s[v] = DegreeSequence.Value[v];

                    var k = 3;
                    for (int i = 0; i < k; i++)
                    {
                        foreach (var v in _vertices)
                        {
                            sp[v] = (int)((s[v] << 13 | s[v] >> 19) ^ 0xff00ff00);
                            foreach (var w in Neighbors[v])
                            {
                                sp[v] += s[w];
                            }
                        }

                        foreach (var v in _vertices)
                            s[v] = sp[v];
                    }

                    return s;
                });

            NodeInvariantTwo = new Lazy<int[]>(() =>
            {
                var s = new int[N];
                foreach (var v in _vertices)
                {
                    s[v] = DegreeSequence.Value[v];

                    foreach (var w in Neighbors[v])
                    {
                        s[v] += 3 * DegreeSequence.Value[w];

                        foreach (var z in Neighbors[w])
                        {
                            if (z != v)
                                s[v] += 7 * DegreeSequence.Value[z];
                        }
                    }
                }

                return s;
            });
        }
        void InitializeLazyLoaders()
        {
            _independentSets = new Lazy<List<List<int>>>(() => IndependentSetsInSubgraph(0).ToList(), true);
            _independentTwoSets = new Lazy<List<List<int>>>(() => _independentSets.Value.Where(set => set.Count == 2).ToList(), true);
            _independentThreeSets = new Lazy<List<List<int>>>(() => _independentSets.Value.Where(set => set.Count == 3).ToList(), true);
            _vertexSubsets = new Lazy<List<List<int>>>(() => ListUtility.GenerateSublists(_vertices));
            _maximalIndependentSets = new Lazy<List<List<int>>>(() => ListUtility.MaximalElements(_independentSets.Value), true);
        }

        #region Equality and Isomorphism
        public override string ToString()
        {
            return string.Join(" ", GetEdgeWeights().Select(x => x.ToString()));
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var H = obj as Graph;
            if (H == null) return false;

            if (N != H.N) return false;
            if (E != H.E) return false;

            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    if (_adjacent[i, j] != H[i, j]) return false;

            return true;
        }

        public static bool operator ==(Graph A, Graph B)
        {
            return (object)A == null && (object)B == null ||
                   (object)A != null && (object)B != null && A.Equals(B);
        }
        public static bool operator !=(Graph A, Graph B)
        {
            return !(A == B);
        }
        public Graph PermuteVertices(Permutation p)
        {
            var adjacent = new bool[N, N];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    adjacent[p[i], p[j]] = _adjacent[i, j];

            List<int> vertexWeight = null;
            if (VertexWeight != null)
            {
                vertexWeight = new List<int>();
                var pp = p.Inverse();

                foreach (var v in _vertices)
                    vertexWeight.Add(VertexWeight[pp[v]]);
            }

            return new Graph(adjacent, vertexWeight);
        }

        static bool MaybeIsomorphic(Graph A, Graph B)
        {
            if (A.N != B.N) return false;
            if (A.E != B.E) return false;

            A.TransitivePartition.Refine(A.DegreeSequence.Value, 1);
            B.TransitivePartition.Refine(B.DegreeSequence.Value, 1);

            A.TransitivePartition.Refine(A.NodeInvariantOne.Value, 2);
            B.TransitivePartition.Refine(B.NodeInvariantOne.Value, 2);

            A.TransitivePartition.Refine(A.NodeInvariantTwo.Value, 3);
            B.TransitivePartition.Refine(B.NodeInvariantTwo.Value, 3);

            if (!A.TransitivePartition.Equals(B.TransitivePartition))
                return false;

            return true;
        }
        public static bool Isomorphic(Graph A, Graph B)
        {
            if (!MaybeIsomorphic(A, B))
                return false;

            return A.Contains(B, induced: true);
        }
        public class IsomorphismComparer : IEqualityComparer<Graph>
        {
            public bool Equals(Graph x, Graph y)
            {
                return Graph.Isomorphic(x, y);
            }
            public int GetHashCode(Graph g)
            {
                return 0;
            }
        }

        public bool Contains(Graph A, bool induced)
        {
            return Contains(A, induced, (_, __, ___, ____) => true);
        }

        public bool Contains(Graph A, bool induced, Func<Graph, Graph, int, int, bool> condition)
        {
            if (N < A.N) return false;

            var tau = new List<int>();
            return Contains(A, induced, condition, new int[A.N], tau, 0);
        }

        bool Contains(Graph A, bool induced, Func<Graph, Graph, int, int, bool> condition, int[] tau, List<int> placed, int v)
        {
            if (v == A.N)
                return true;

            var images = placed.Select(u => tau[u]).OrderBy(t => t).ToList();
            var requiredNeighbors = A.Neighbors[v].IntersectionSorted(placed).Select(u => tau[u]).OrderBy(t => t).ToList();
            var candidates = Vertices.Except(images)
                                     .Where(w => A.Degree(v) <= Degree(w))
                                     .Where(w => induced ? requiredNeighbors.SequenceEqual(Neighbors[w].Intersection(images))
                                                         : requiredNeighbors.SubsetEqualSorted(Neighbors[w]))
                                     .Where(w => condition(this, A, w, v));

            foreach (var candidate in candidates)
            {
                tau[v] = candidate;
                placed.Add(v);

                if (Contains(A, induced, condition, tau, placed, v + 1))
                    return true;

                placed.RemoveAt(placed.Count - 1);
            }

            return false;
        }

        public bool ContainsInduced(Graph A)
        {
            return Contains(A, induced: true);
        }

        public bool ContainsInducedOld(Graph A)
        {
            if (N < A.N) return false;

            var vertexSets = _vertexSubsets.Value;

            foreach (var vertices in vertexSets)
                if (Graph.Isomorphic(InducedSubgraph(vertices), A)) return true;

            return false;
        }
        public IEnumerable<List<int>> EnumerateConnectedOrderings(List<int> initialVertices)
        {
            var done = true;
            foreach (var v in Vertices.Except(initialVertices))
            {
                done = false;
                if (DegreeInSubgraph(v, initialVertices) > 0)
                {
                    initialVertices.Add(v);
                    foreach (var ordering in EnumerateConnectedOrderings(initialVertices))
                        yield return ordering.ToList();
                    initialVertices.RemoveAt(initialVertices.Count - 1);
                }
            }

            if (done)
                yield return initialVertices;
        }

        public bool IsSpanningSubgraphOf(Graph A)
        {
            if (N != A.N) return false;

            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    if (_adjacent[i, j] && !A[i, j]) return false;

            return true;
        }
        #endregion

        #region Export
        public List<int> GetEdgeWeights()
        {
            return GetEdgeWeights(new int[] { }, new Tuple<int, int>[] { });
        }

        public Graph InducedSubgraph(List<int> subgraph)
        {
            List<int> vertexWeight = null;
            if (VertexWeight != null)
            {
                vertexWeight = new List<int>(subgraph.Count);
                foreach (var v in subgraph)
                {
                    vertexWeight.Add(VertexWeight[v]);
                }
            }

            return new Graph(GetEdgeWeights(subgraph), vertexWeight);
        }
        public List<int> GetEdgeWeights(List<int> subgraph)
        {
            var n = subgraph.Count;
            var w = new List<int>(n);

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    w.Add(0);

                    if (_adjacent[subgraph[i], subgraph[j]] || _adjacent[subgraph[j], subgraph[i]])
                        w[k] = 1;

                    k++;
                }
            }

            return w;
        }

        public List<int> GetEdgeWeights(bool[,] adjacent, bool[,] directed)
        {
            return GetEdgeWeights(adjacent, directed, new int[] { }, new Tuple<int, int>[] { });
        }

        List<int> GetEdgeWeights(IEnumerable<int> removedVertices, IEnumerable<Tuple<int, int>> removedEdges)
        {
            return GetEdgeWeights(_adjacent, Directed, removedVertices, removedEdges);
        }

        List<int> GetEdgeWeights(bool[,] adjacent, bool[,] directed, IEnumerable<int> removedVertices, IEnumerable<Tuple<int, int>> removedEdges)
        {
            var w = new List<int>();

            int k = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    w.Add(0);

                    if (!removedVertices.Any(v => v == i || v == j) && !removedEdges.Any(e => e.Item1 == i && e.Item2 == j || e.Item1 == j && e.Item2 == i))
                    {
                        if (adjacent[i, j] || adjacent[j, i])
                        {
                            w[k] = 1;
                            if (directed[j, i])
                                w[k] = -1;
                        }
                    }

                    k++;
                }
            }

            return w;
        }

        static readonly List<string> DotColors = new List<string>() { "cadetblue", "brown", "dodgerblue", "turquoise", "orchid", "blue", "red", "green", 
                                                                      "yellow", "cyan",
                                                                      "limegreen",  "pink", 
                                                                      "orange",  "goldenrod", "aquamarine", "black", };

        static readonly List<string> TikzColors = new List<string>() { "blue", "red", "green", 
                                                                      "Orchid", "yellow", "cyan", "Turquoise", 
                                                                      "CadetBlue", "LimeGreen", "brown", "pink", 
                                                                      "orange", "Cerulean", "Goldenrod", "Aquamarine", "black", };

        public string ToDotWithFactors(bool labelEdges = false)
        {
            var sb = new StringBuilder();

            var factors = Factor();
            var colorRanges = new List<IEnumerable<int>>();
            int last = 0;
            var G = Graphs.Empty;
            foreach (var factor in factors)
            {
                colorRanges.Add(Enumerable.Range(last, factor.N));
                last += factor.N;

                G *= factor;
            }

            sb.AppendLine("graph G {");
            sb.AppendLine("overlap = false;");
            sb.AppendLine("splines=true;");
            sb.AppendLine("sep=0.3;");
            sb.AppendLine("node[fontsize=20, style=bold, color=black; shape=circle, penwidth=1];");
            sb.AppendLine("edge[style=bold, color=black, penwidth=2];");
            foreach (int v in _vertices)
            {
                int colorIndex = colorRanges.Select((r, i) => new { range = r, index = i }).First(p => p.range.Contains(v)).index % DotColors.Count;
                var label = v.ToString();
                sb.AppendLine(string.Format(@"{0} [label = ""{2}"", style = filled, fillcolor = ""{1}""];", v, DotColors[colorIndex], label));
            }

            int k = 0;
            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                {
                    if (G._adjacent[i, j])
                    {
                        if (labelEdges)
                            sb.AppendLine(string.Format(@"{0} -- {1} [label = ""{2}""]", i, j, k));
                        else
                            sb.AppendLine(string.Format("{0} -- {1}", i, j));

                        k++;
                    }
                }
            sb.AppendLine("}");

            return sb.ToString();
        }

        public string ToDotForTikz()
        {
            var sb = new StringBuilder();

            var factors = Factor();
            var colorRanges = new List<IEnumerable<int>>();
            int last = 0;
            var G = Graphs.Empty;
            foreach (var factor in factors)
            {
                colorRanges.Add(Enumerable.Range(last, factor.N));
                last += factor.N;

                G *= factor;
            }

            sb.AppendLine("graph G {");
            foreach (int v in _vertices)
            {
                int colorIndex = colorRanges.Select((r, i) => new { range = r, index = i }).First(p => p.range.Contains(v)).index % TikzColors.Count;
                sb.AppendLine(string.Format(@"{0} [label = """", style = ""draw, circle, fill={1}""];", v, TikzColors[colorIndex]));
            }

            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                {
                    if (G._adjacent[i, j])
                        sb.AppendLine(string.Format("{0} -- {1}", i, j));
                }
            sb.AppendLine("}");

            return sb.ToString();
        }
        #endregion

        #region Operations
        bool IsProperFactor(List<int> subset)
        {
            if (subset.Count <= 0) return true;
            if (subset.Count >= N) return false;

            var rest = ListUtility.Difference(_vertices, subset);

            var pairs = from i in subset
                        from j in rest
                        select new Tuple<int, int>(i, j);

            foreach (var pair in pairs)
                if (!_adjacent[pair.Item1, pair.Item2]) return false;

            return true;
        }
        public IEnumerable<Graph> Factor()
        {
            var properFactors = _vertexSubsets.Value.Where(vertices => IsProperFactor(vertices)).ToList();
            var maximalFactor = ListUtility.MaximalElements(properFactors)[0];
            if (maximalFactor.Count <= 0) return new List<Graph>() { Clone() };

            var rest = ListUtility.Difference(_vertices, maximalFactor);
            var maxFactor = InducedSubgraph(maximalFactor);
            var restGraph = InducedSubgraph(rest);
            return maxFactor.Factor().Concat(restGraph.Factor());
        }
        public Graph Clone()
        {
            return new Graph(GetEdgeWeights());
        }
        public Graph Complement()
        {
            var adjacent = new bool[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                {
                    if (i == j) continue;
                    adjacent[i, j] = !_adjacent[i, j];
                }

            return new Graph(adjacent);
        }
        public Graph RemoveVertex(int v)
        {
            var subgraph = Vertices.ToList();
            subgraph.Remove(v);
            return InducedSubgraph(subgraph);
        }
        public Graph RemoveEdge(Tuple<int, int> e)
        {
            return new Graph(GetEdgeWeights(new int[] { }, new Tuple<int, int>[] { e }));
        }
        public Graph RemoveEdge(int v1, int v2)
        {
            return RemoveEdge(new Tuple<int, int>(v1, v2));
        }
        public Graph AddEdge(int v1, int v2, bool directed)
        {
            var g = Clone();
            g._adjacent[v1, v2] = g._adjacent[v2, v1] = true;
            if (directed)
                g.Directed[v1, v2] = true;

            return new Graph(g.GetEdgeWeights());
        }
        Tuple<int, int> GetArbitraryEdge()
        {
            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    if (_adjacent[i, j])
                        return new Tuple<int, int>(i, j);

            return null;
        }
        List<Tuple<int, int>> GetEdgeTuples()
        {
            var tuples = new List<Tuple<int, int>>();

            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    if (_adjacent[i, j])
                    {
                        if (Directed[i, j])
                            tuples.Add(new Tuple<int, int>(i, j));
                        else
                            tuples.Add(new Tuple<int, int>(j, i));
                    }

            return tuples;
        }
        public Graph DisjointUnion(Graph H)
        {
            int n = N + H.N;

            var adjacent = new bool[n, n];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    adjacent[i, j] = _adjacent[i, j];

            for (int i = 0; i < H.N; i++)
                for (int j = 0; j < H.N; j++)
                    adjacent[N + i, N + j] = H[i, j];

            return new Graph(adjacent);
        }
        public Graph Join(Graph H)
        {
            var G = DisjointUnion(H);

            for (int i = 0; i < N; i++)
                for (int j = 0; j < H.N; j++)
                {
                    G._adjacent[i, N + j] = true;
                    G._adjacent[N + j, i] = true;
                }

            G.Initialize();
            return G;
        }
        public Graph AddEdge(int x, int y)
        {
            var G = Clone();
            G._adjacent[x, y] = true;
            G._adjacent[y, x] = true;

            G.Initialize();
            return G;
        }
        public Graph AttachNewVertex(params int[] neighbors)
        {
            return AttachNewVertex((IList<int>)neighbors);
        }
        public Graph AttachNewVertex(IList<int> neighbors)
        {
            var adjacent = new bool[N + 1, N + 1];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    adjacent[i, j] = _adjacent[i, j];

            foreach (var neighbor in neighbors)
            {
                adjacent[N, neighbor] = true;
                adjacent[neighbor, N] = true;
            }

            return new Graph(adjacent);
        }
        public static Graph operator +(Graph A, Graph B)
        {
            return A.DisjointUnion(B);
        }
        public static Graph operator *(Graph A, Graph B)
        {
            return A.Join(B);
        }
        public static Graph operator *(int k, Graph A)
        {
            var G = A.Clone();
            for (int i = 1; i < k; i++)
                G += A;

            G.Initialize();
            return G;
        }

        public Graph Square()
        {
            var g = Clone();
            foreach (var v in Vertices)
            {
                foreach (var pair in Neighbors[v].CartesianProduct(Neighbors[v]))
                {
                    if (pair.Item1 != pair.Item2)
                        g._adjacent[pair.Item1, pair.Item2] = g._adjacent[pair.Item2, pair.Item1] = true;
                }
            }

            return g.Clone();
        }
        #endregion

        #region Edges
        public int Size
        {
            get
            {
                var t = 0;
                for (int i = 0; i < N; i++)
                    t += OutDegree(i);

                return t;
            }
        }
        public int MaxDegree
        {
            get
            {
                int max = 0;
                for (int i = 0; i < N; i++)
                    max = Math.Max(max, Degree(i));

                return max;
            }
        }
        public int MinDegree
        {
            get
            {
                int min = int.MaxValue;
                for (int i = 0; i < N; i++)
                    min = Math.Min(min, Degree(i));

                return min;
            }
        }
        public int MaxOutDegree
        {
            get
            {
                int max = 0;
                for (int i = 0; i < N; i++)
                    max = Math.Max(max, OutDegree(i));

                return max;
            }
        }
        public int MaxInDegree
        {
            get
            {
                int max = 0;
                for (int i = 0; i < N; i++)
                    max = Math.Max(max, InDegree(i));

                return max;
            }
        }

        public bool IsRegular()
        {
            if (N <= 0)
                return true;

            return Neighbors.All(n => n.Count == Neighbors[0].Count);
        }

        public int Degree(int v)
        {
            return Neighbors[v].Count;
        }
        public int OutDegree(int v)
        {
            return OutNeighbors[v].Count;
        }
        public int InDegree(int v)
        {
            return InNeighbors[v].Count;        }

        public int DegreeInSubgraph(int v, List<int> subgraph)
        {
            return ListUtility.IntersectionCountSorted(Neighbors[v], subgraph);
        }

        public List<int> NeighborsInSubgraph(int v, List<int> subgraph)
        {
            return ListUtility.IntersectionSorted(Neighbors[v], subgraph);
        }

        public int SumOverSubgraphNeighbors(int v, List<int> subgraph, int[] g)
        {
            return ListUtility.IntersectionSorted(Neighbors[v], subgraph).Sum(w => g[w]);
        }

        public Func<int, int> GetDegreeMinusKFunc(int k)
        {
            return v => Degree(v) - k;
        }

        public int EdgesOn(List<int> A)
        {
            int edges = 0;
            for (int i = 0; i < A.Count; i++)
                for (int j = i + 1; j < A.Count; j++)
                    if (_adjacent[A[i], A[j]]) edges++;

            return edges;
        }

        public List<int> EdgeIndicesOn(List<int> A)
        {
            var edgeIndices = new List<int>();

            int k = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (_adjacent[i, j])
                    {
                        if (A.Contains(i) && A.Contains(j))
                            edgeIndices.Add(k);

                        k++;
                    }
                }
            }

            return edgeIndices;
        }
        public int EdgesBetween(List<int> A, List<int> B)
        {
            var intersection = ListUtility.Intersection(A, B);
            A = ListUtility.Difference(A, B);

            int edges = 0;
            for (int i = 0; i < A.Count; i++)
                for (int j = 0; j < B.Count; j++)
                    if (_adjacent[A[i], B[j]]) edges++;

            return edges + EdgesOn(intersection);
        }
        #endregion

        #region Independent sets
     
        IEnumerable<List<int>> IndependentSetsInSubgraph(int firstVertex)
        {
            if (firstVertex >= N) return new List<List<int>>() { new List<int>() };

            var subgraphSets = IndependentSetsInSubgraph(firstVertex + 1);

            return subgraphSets.Union(subgraphSets.Where(set => IsIndependentOf(firstVertex, set))
                                                    .Select(set =>
                                                    {
                                                        var bigger = new List<int>() { firstVertex };
                                                        bigger.AddRange(set);
                                                        return bigger;
                                                    }));
        }
        bool IsIndependentOf(int v, List<int> set)
        {
            foreach (var w in set) if (w == v || _adjacent[w, v]) return false;

            return true;
        }
        public bool IsClique(List<int> subset)
        {
            for (int i = 0; i < subset.Count; i++)
                for (int j = i + 1; j < subset.Count; j++)
                    if (!_adjacent[subset[i], subset[j]]) return false;

            return true;
        }
        public bool IsIndependent(List<int> subset)
        {
            for (int i = 0; i < subset.Count; i++)
                for (int j = i + 1; j < subset.Count; j++)
                    if (_adjacent[subset[i], subset[j]]) return false;

            return true;
        }
        public bool IsComplete()
        {
            return IsClique(_vertices);
        }
        public bool IsCliqueNumberAtLeast(int k)
        {
            if (k > _vertices.Count)
                return false;
            foreach (var set in ListUtility.EnumerateSublists(_vertices, k))
            {
                if (IsClique(set))
                    return true;
            }

            return false;
        }

        public int IndependenceNumber()
        {
            return IndependenceNumber(Vertices);
        }
        public int IndependenceNumber(IEnumerable<int> subgraph)
        {
            return _independentSets.Value.Max(I => I.IntersectionCount(subgraph));
        }
        public List<int> MaximumIndependentSubset(IEnumerable<int> subgraph)
        {
            var i = IndependenceNumber(subgraph);
            return _independentSets.Value.First(I => I.IntersectionCount(subgraph) == i);
        }

        public IEnumerable<List<int>> EnumerateMaximalIndependentSets(List<int> set)
        {
            return EnumerateBronKerbosch(set, new List<int>(), new List<int>());
        }

        public IEnumerable<List<int>> EnumerateMaximalIndependentSets()
        {
            return EnumerateBronKerbosch(Vertices, new List<int>(), new List<int>());
        }

        IEnumerable<List<int>> EnumerateBronKerbosch(List<int> P, List<int> R, List<int> X)
        {
            if (P.Count == 0 && X.Count == 0)
                yield return R.ToList();
            else
            {
                var PC = P.ToList();
                var XC = X.ToList();

                var u = TomitaPivot(P, X);
                foreach (var v in P.Except(ComplementNeighbors[u]))
                {
                    R.Add(v);
                    foreach (var set in EnumerateBronKerbosch(PC.Intersection(ComplementNeighbors[v]), R, XC.Intersection(ComplementNeighbors[v])))
                        yield return set;

                    R.Remove(v);
                    PC.Remove(v);
                    XC.Add(v);
                }
            }
        }

        int TomitaPivot(List<int> P, List<int> X)
        {
            var max = -1;
            var best = -1;
            foreach (var u in P.Concat(X))
            {
                var n = ComplementNeighbors[u].IntersectionCount(P);
                if (n > max)
                {
                    max = n;
                    best = u;
                }
            }

            return best;
        }

        public List<List<int>> FindChiColoring()
        {
            for (int K = 0; K < int.MaxValue; K++)
            {
                var c = FindKColoring(Vertices, K);
                if (c != null)
                    return c;
            }

            return null;
        }

        public List<List<int>> FindKColoring(List<int> subgraph, int K)
        {
            if (subgraph.Count <= 0)
                return new List<List<int>>();
            if (K <= 0)
                return null;

            foreach (var I in EnumerateMaximalIndependentSets(subgraph))
            {
                var c = FindKColoring(subgraph.Difference(I), K - 1);
                if (c != null)
                {
                    c.Insert(0, I);
                    return c;
                }
            }

            return null;
        }

        #endregion

        #region List coloring

        public int MaxColorableSubset(List<long> assignment, List<int> subset)
        {
            return MaxColorableSubset(assignment, 0, subset);
        }

        int MaxColorableSubset(List<long> assignment, int v, List<int> subset)
        {
            if (v >= subset.Count)
                return 0;

            var max = MaxColorableSubset(assignment, v + 1, subset);

            var colors = assignment[subset[v]];
            while (colors != 0)
            {
                var color = colors & -colors;

                var assignmentCopy = new List<long>(assignment);
                foreach (var neighbor in _laterNeighbors.Value[subset[v]])
                    assignmentCopy[neighbor] &= ~color;

                max = Math.Max(max, 1 + MaxColorableSubset(assignmentCopy, v + 1, subset));
                if (max >= subset.Count)
                    break;

                colors ^= color;
            }

            return max;
        }

        public int GreedyColor(List<long> assignment, List<int> subset)
        {
            var coloring = new long[N];
            foreach (var v in subset)
            {
                var lost = Neighbors[v].Aggregate(0L, (l, x) => l | coloring[x]);
                var colors = assignment[v] & ~lost;
                coloring[v] = colors & -colors;
            }

            return coloring.Count(c => c != 0);
        }

        public bool IsChoosable(List<long> assignment, List<int> subset)
        {
            return IsChoosable(assignment, 0, subset);
        }
        bool IsChoosable(List<long> assignment, int v, List<int> subset)
        {
            if (v >= subset.Count)
                return true;

            var colors = assignment[subset[v]];
            while (colors != 0)
            {
                var color = colors & -colors;

                var assignmentCopy = new List<long>(assignment);
                foreach (var neighbor in _laterNeighbors.Value[subset[v]])
                    assignmentCopy[neighbor] &= ~color;

                if (IsChoosable(assignmentCopy, v + 1, subset))
                    return true;

                colors ^= color;
            }

            return false;
        }
        public bool IsChoosable(List<long> assignment)
        {
            return IsChoosable(assignment, 0);
        }
        bool IsChoosable(List<long> assignment, int v)
        {
            if (v >= N)
                return true;

            var colors = assignment[v];
            while (colors != 0)
            {
                var color = colors & -colors;

                var assignmentCopy = new List<long>(assignment);
                foreach (var neighbor in _laterNeighbors.Value[v])
                    assignmentCopy[neighbor] &= ~color;

                if (IsChoosable(assignmentCopy, v + 1))
                    return true;

                colors ^= color;
            }

            return false;
        }
        public bool IsCliqueCovering(List<long> assignment)
        {
            foreach (var set in _independentTwoSets.Value) 
                if ((assignment[set[0]] & assignment[set[1]]) != 0) 
                    return false;

            return true;
        }

        public List<List<int>> CheckFGChoosable(Func<int, int> f, Func<int, int> g)
        {
            var pot = Enumerable.Range(0, Vertices.Sum(v => g(v)) - 1).ToList();
            var first = Enumerable.Range(0, f(0)).ToList();
            foreach (var L in ((IEnumerable<IEnumerable<List<int>>>)(new[] { (new[] { first }) })).Concat(Vertices.Skip(1).Select(v => ListUtility.EnumerateSublists(pot, f(v)))).CartesianProduct())
            {
                var LL = L.ToList();
                if (!IsGColorable(LL, g))
                    return LL;
            }

            return null;
        }

        bool IsGColorable(List<List<int>> L, Func<int, int> g)
        {
            foreach (var coloring in Vertices.Select(v => ListUtility.EnumerateSublists(L[v], g(v))).CartesianProduct())
            {
                if (IsProperColoring(coloring.ToList()))
                    return true;
            }

            return false;
        }

        bool IsProperColoring(List<List<int>> coloring)
        {
            for (int v = 0; v < N; v++)
            {
                for (int w = v + 1; w < N; w++)
                {
                    if (!_adjacent[v, w])
                        continue;

                    if (coloring[v].IntersectionCount(coloring[w]) > 0)
                        return false;
                }
            }

            return true;
        } 
        #endregion

        #region Online list coloring
        public int Mic()
        {
            var mic = 0;
            foreach (var set in EnumerateMaximalIndependentSets())
            {
                var t = set.Sum(v => Degree(v));
                mic = Math.Max(mic, t);
            }

            return mic;
        }
        public bool IsOnlineFChoosable(Func<int, int> f)
        {
            NodesVisited = 0;
            CacheHits = 0;

            var fTrace = new int[N];
            foreach (var v in _vertices)
                fTrace[v] = f(v);

            var cache = new Dictionary<OnlineChoiceHashGraph, bool>();

            return IsOnlineFChoosable(fTrace, Enumerable.Repeat(1, N).ToArray(), cache);
        }
        bool IsOnlineFChoosable(int[] f, int[] g, Dictionary<OnlineChoiceHashGraph, bool> cache)
        {
            NodesVisited++;

            OnlineChoiceHashGraph key = null;

            var result = true;
            int[] gClone = null;

            var liveVertices = g.Select((v, i) => v > 0 ? i : -1).Where(i => i >= 0).ToList();
            var freebies = liveVertices.Where(v => DegreeInSubgraph(v, liveVertices) < f[v]).ToList();

            while (freebies.Count > 0)
            {
                if (gClone == null)
                {
                    gClone = new int[g.Length];
                    Array.Copy(g, gClone, g.Length);
                }

                foreach (var v in freebies)
                    g[v] = 0;

                liveVertices = g.Select((v, i) => v > 0 ? i : -1).Where(i => i >= 0).ToList();
                freebies = liveVertices.Where(v => DegreeInSubgraph(v, liveVertices) < f[v]).ToList();
            }

            if (liveVertices.Count <= 0)
            {
                result = true;
                goto done;
            }
            if (liveVertices.Any(v => f[v] <= 0))
            {
                result = false;
                goto done;
            }

            key = new OnlineChoiceHashGraph(f, g);
            bool cachedResult;
            if (cache.TryGetValue(key, out cachedResult))
            {
                CacheHits++;

                key = null;
                result = cachedResult;
                goto done;
            }

            var independentSets = _independentSets.Value.Where(set => set.Count >= 1 && ListUtility.SubsetEqualSorted(set, liveVertices)).ToList();

            foreach (var V in ListUtility.EnumerateSublists(liveVertices))
            {
                if (V.Count <= 1)
                    continue;

                if (independentSets.Any(ss => ListUtility.SubsetEqualSorted(V, ss)))
                    continue;

                foreach (var v in V)
                    f[v]--;

                var maximalIndependentSets = ListUtility.MaximalElementsSorted(independentSets.Where(set => ListUtility.SubsetEqualSorted(set, V)).ToList());

                var choosable = false;
                foreach (var C in maximalIndependentSets)
                {
                    foreach (var v in C)
                        g[v]--;

                    choosable = IsOnlineFChoosable(f, g, cache);

                    foreach (var v in C)
                        g[v]++;

                    if (choosable)
                        break;
                }

                foreach (var v in V)
                    f[v]++;

                if (!choosable)
                {
                    result = false;
                    goto done;
                }
            }

        done:

            if (gClone != null)
                Array.Copy(gClone, g, g.Length);

            if (key != null)
                cache[key] = result;

            return result;
        }
        #region online (f:g)-choosability
        public bool IsOnlineFGChoosable(Func<int, int> f, Func<int, int> g)
        {
            NodesVisited = 0;
            CacheHits = 0;

            var cache = new Dictionary<OnlineChoiceHashGraph, bool>();
            return IsOnlineFGChoosable(_vertices.Select(v => f(v)).ToArray(), _vertices.Select(v => g(v)).ToArray(), cache);
        }
        bool IsOnlineFGChoosable(int[] f, int[] g, Dictionary<OnlineChoiceHashGraph, bool> cache)
        {
            NodesVisited++;
            OnlineChoiceHashGraph key = null;

            var result = true;
            int[] gClone = new int[g.Length];
            Array.Copy(g, gClone, g.Length);

            while (true)
            {
                var changed = false;
                foreach(var v in _vertices)
                {
                    if (g[v] <= 0)
                        continue;

                    var need = g[v] + Neighbors[v].Sum(w => g[w]);
                    if (f[v] >= need)
                    {
                        g[v] = 0;
                        changed = true;
                    }
                }

                if (!changed)
                    break;
            }

            var liveVertices = g.IndicesWhere(v => v > 0).ToList();
            if (liveVertices.Count <= 0)
            {
                result = true;
                goto done;
            }
            if (liveVertices.Any(v => f[v] < g[v]))
            {
                result = false;
                goto done;
            }

            key = new OnlineChoiceHashGraph(f, g);
            bool cachedResult;
            if (cache.TryGetValue(key, out cachedResult))
            {
                CacheHits++;

                key = null;
                result = cachedResult;
                goto done;
            }

            var independentSets = _independentSets.Value.Where(set => set.Count >= 1 && ListUtility.SubsetEqualSorted(set, liveVertices)).ToList();

            foreach (var V in ListUtility.EnumerateSublists(liveVertices))
            {
                if (V.Count <= 1)
                    continue;

                if (independentSets.Any(ss => ListUtility.SubsetEqualSorted(V, ss)))
                    continue;

                foreach (var v in V)
                    f[v]--;

                var maximalIndependentSets = ListUtility.MaximalElementsSorted(independentSets.Where(set => ListUtility.SubsetEqualSorted(set, V)).ToList());

                var choosable = false;
                foreach (var C in maximalIndependentSets)
                {
                    foreach (var v in C)
                        g[v]--;

                    choosable = IsOnlineFGChoosable(f, g, cache);

                    foreach (var v in C)
                        g[v]++;

                    if (choosable)
                        break;
                }

                foreach (var v in V)
                    f[v]++;

                if (!choosable)
                {
                    result = false;
                    goto done;
                }
            }

        done:

            if (gClone != null)
                Array.Copy(gClone, g, g.Length);

            if (key != null)
                cache[key] = result;

            return result;
        }
        #endregion
        #endregion

        #region Orientations
        public int Direction(Tuple<int, int> e)
        {
            if (!_adjacent[e.Item1, e.Item2])
                return 0;

            return Directed[e.Item2, e.Item1] ? -1 : 1;
        }
        bool MatchesOutMinusIn(List<int> outMinusIn)
        {
            return _vertices.All(v => OutDegree(v) - InDegree(v) == outMinusIn[v]);
        }
        public static bool MemoizeEulerianSubgraphCounting { get; set; }
        public List<Graph> SpanningOutMinusInSubgraphs(List<int> outMinusIn, List<Tuple<int, int>> edges, Dictionary<OrientationHashGraph, List<Graph>> spanningEulerianOrientationsCache)
        {
            NodesVisited++;

            var graphs = new List<Graph>();

            if (_vertices.Any(v => outMinusIn[v] > OutDegree(v) || -outMinusIn[v] > InDegree(v)))
                return graphs;

            var key = new OrientationHashGraph(this, outMinusIn);

            if (MemoizeEulerianSubgraphCounting)
            {
                List<Graph> spanners;
                if (spanningEulerianOrientationsCache.TryGetValue(key, out spanners))
                {
                    CacheHits++;
                    return spanners;
                }
            }

            if (edges.Count <= 0)
            {
                if (MatchesOutMinusIn(outMinusIn))
                    graphs.Add(this);

                return graphs;
            }

            var e = edges.Last();

            edges.Remove(e);
            var g = RemoveEdge(e);

            graphs.AddRange(g.SpanningOutMinusInSubgraphs(outMinusIn, edges, spanningEulerianOrientationsCache));

            var outMinusInPrime = outMinusIn.ToList();
            var v1 = e.Item1;
            var v2 = e.Item2;

            if (Direction(e) == 1)
            {
                outMinusInPrime[e.Item1]--;
                outMinusInPrime[e.Item2]++;
            }
            else
            {
                outMinusInPrime[e.Item1]++;
                outMinusInPrime[e.Item2]--;

                v1 = e.Item2;
                v2 = e.Item1;
            }

            graphs.AddRange(g.SpanningOutMinusInSubgraphs(outMinusInPrime, edges, spanningEulerianOrientationsCache).Select(graph => graph.AddEdge(v1, v2, true)));

            if (MemoizeEulerianSubgraphCounting)
                spanningEulerianOrientationsCache[key] = graphs;

            edges.Add(e);
            return graphs;
        }
        public List<Graph> SpanningEulerianSubgraphs(List<Tuple<int, int>> edges, Dictionary<OrientationHashGraph, List<Graph>> spanningEulerianOrientationsCache)
        {
            return SpanningOutMinusInSubgraphs(new int[N].ToList(), edges, spanningEulerianOrientationsCache);
        }
        public void CountSpanningEulerianSubgraphs(out int even, out int odd)
        {
            CacheHits = 0;
            NodesVisited = 0;
            var spanningEulerianOrientationsCache = new Dictionary<OrientationHashGraph, List<Graph>>();
            var edges = GetEdgeTuples();

            even = 0;
            odd = 0;
            foreach (var g in SpanningEulerianSubgraphs(edges, spanningEulerianOrientationsCache))
            {
                if (g.Size % 2 == 0)
                    even++;
                else
                    odd++;
            }
        }
        public void CountSpanningEulerianSubgraphsUsingVertices(List<int> vertices, out int even, out int odd)
        {
            CacheHits = 0;
            NodesVisited = 0;

            if (vertices == null)
                vertices = new List<int>();

            var spanningEulerianOrientationsCache = new Dictionary<OrientationHashGraph, List<Graph>>();
            var edges = GetEdgeTuples();

            even = 0;
            odd = 0;
            foreach (var g in SpanningEulerianSubgraphs(edges, spanningEulerianOrientationsCache))
            {
                foreach (var v in vertices)
                {
                    if (g.Degree(v) <= 0)
                        goto skip;
                }

                if (g.Size % 2 == 0)
                    even++;
                else
                    odd++;

            skip: ;
            }
        }
        public void CountSpanningEulerianSubgraphsUsingEdges(List<Tuple<int, int>> edges, out int even, out int odd)
        {
            CacheHits = 0;
            NodesVisited = 0;

            if (edges == null)
                edges = new List<Tuple<int, int>>();

            var spanningEulerianOrientationsCache = new Dictionary<OrientationHashGraph, List<Graph>>();
            var edgeTuples = GetEdgeTuples();

            even = 0;
            odd = 0;
            foreach (var g in SpanningEulerianSubgraphs(edgeTuples, spanningEulerianOrientationsCache))
            {
                foreach (var e in edges)
                {
                    if (!g._adjacent[e.Item1, e.Item2])
                        goto skip;
                }

                if (g.Size % 2 == 0)
                    even++;
                else
                    odd++;

            skip: ;
            }
        }
        public IEnumerable<Graph> EnumerateOrientations(Func<int, int> requiredIndegree)
        {
            var gain = new int[N];

            foreach (var inNeighborList in EnumerateOrientationsFor(0, requiredIndegree, gain))
            {
                var adjacent = new bool[N, N];
                var directed = new bool[N, N];

                for (int i = 0; i < inNeighborList.Count; i++)
                {
                    var v = inNeighborList.Count - 1 - i;

                    foreach (var w in inNeighborList[i])
                    {
                        adjacent[v, w] = true;
                        adjacent[w, v] = true;

                        directed[w, v] = true;
                    }

                    var outNeighbors = ListUtility.Difference(_laterNeighbors.Value[v], inNeighborList[i]);

                    foreach (var w in outNeighbors)
                    {
                        adjacent[v, w] = true;
                        adjacent[w, v] = true;

                        directed[v, w] = true;
                    }
                }

                yield return new Graph(GetEdgeWeights(adjacent, directed));
            }
        }
        IEnumerable<List<List<int>>> EnumerateOrientationsFor(int v, Func<int, int> requiredIndegree, int[] gain)
        {
            if (v >= N - 1)
            {
                if (gain[v] < requiredIndegree(v))
                    yield break;
                else 
                    yield return new List<List<int>>();
            }
            else
            {
                foreach (var ins in ListUtility.EnumerateSublists(_laterNeighbors.Value[v]))
                {
                    if (ins.Count < requiredIndegree(v) - gain[v])
                        continue;

                    var outs = ListUtility.Difference(_laterNeighbors.Value[v], ins);

                    foreach (var x in outs)
                        gain[x]++;

                    foreach (var o in EnumerateOrientationsFor(v + 1, requiredIndegree, gain))
                    {
                        o.Add(ins);

                        yield return o;
                    }

                    foreach (var x in outs)
                        gain[x]--;
                }
            }
        }
        public IEnumerable<Graph> EnumerateOrientations(bool fast = false)
        {
            var undirectedWeights = GetEdgeWeights().Select(w => Math.Abs(w)).ToList();
            var edgeIndices = undirectedWeights.Select((w, i) => w != 0 ? i : -1).Where(x => x > 0).ToList();

            var undirected = new Graph(undirectedWeights);

            foreach (var list in ListUtility.EnumerateSublists(edgeIndices))
            {
                var weights = undirectedWeights.Select((w, i) => list.Contains(i) ? -w : w).ToList();

                yield return new Graph(weights);
            }
        }
        public Graph GenerateRandomOrientation()
        {
            var undirectedWeights = GetEdgeWeights().Select(w => Math.Abs(w)).ToList();
            var edgeIndices = undirectedWeights.Select((w, i) => w != 0 ? i : -1).Where(x => x > 0).ToList();

            var list = new List<int>(E / 2);
            foreach (var i in edgeIndices)
            {
                if (RNG.Next(2) == 0)
                    list.Add(i);
            }

            var weights = undirectedWeights.Select((w, i) => list.Contains(i) ? -w : w).ToList();

            return new Graph(weights);
        }
        public IEnumerable<Graph> EnumerateAcyclicOrientations()
        {
            return EnumerateAcyclicOrientations(0, new Dictionary<int,List<int>>(N));
        }
        IEnumerable<Graph> EnumerateAcyclicOrientations(int i, Dictionary<int, List<int>> outNeighbors)
        {
            if (i >= N)
                yield return FromOutNeighborLists(outNeighbors);
            else
            {
                var neighbors = NeighborsInSubgraph(i, Enumerable.Range(0, i).ToList());
                if (neighbors.Count <= 0)
                {
                    var clone = CloneOutNeighbors(outNeighbors);

                    clone[i] = new List<int>();
                    foreach (var g in EnumerateAcyclicOrientations(i + 1, clone))
                        yield return g;
                }
                else
                {
                    var sortedNeighbors = TopologicalSort(i, neighbors, outNeighbors);
                    foreach (var d in EnumerateLegalDirectionAssignments(i, sortedNeighbors, outNeighbors))
                    {
                        var clone = CloneOutNeighbors(outNeighbors);

                        clone[i] = d.EnumerateBits().Select(b => sortedNeighbors[sortedNeighbors.Count - 1 - b]).ToList();
                        foreach (var v in neighbors.Difference(clone[i]))
                            clone[v].Add(i);

                        foreach (var g in EnumerateAcyclicOrientations(i + 1, clone))
                            yield return g;
                    }
                }
            }
        }
        Dictionary<int, List<int>> CloneOutNeighbors(Dictionary<int, List<int>> outNeighbors)
        {
            var clone = new Dictionary<int, List<int>>(outNeighbors.Count);

            foreach (var kvp in outNeighbors)
                clone[kvp.Key] = kvp.Value.ToList();

            return clone;
        }
        List<int> TopologicalSort(int i, List<int> vertices, Dictionary<int, List<int>> outNeighbors)
        {
            var inNeighborCount = new int[i];
            foreach (var kvp in outNeighbors)
            {
                if (!vertices.Contains(kvp.Key))
                    continue;

                foreach (var v in kvp.Value.Intersect(vertices))
                    inNeighborCount[v]++;
            }

            var sorted = new List<int>(i);
            var sources = vertices.Where(v => inNeighborCount[v] == 0).ToList();

            while (sources.Count > 0)
            {
                var source = sources[sources.Count - 1];
                sources.RemoveAt(sources.Count - 1);

                sorted.Add(source);
                foreach (var v in outNeighbors[source].Intersect(vertices))
                {
                    inNeighborCount[v]--;
                    if (inNeighborCount[v] == 0)
                        sources.Add(v);
                }
            }

            return sorted;
        }
        IEnumerable<long> EnumerateLegalDirectionAssignments(int i, List<int> sortedNeighbors, Dictionary<int, List<int>> outNeighbors)
        {
            long d = 0;

            var ancestorLists = GetAncestorLists(i, outNeighbors);
            while (true)
            {
                yield return d;

                if (d.AllSet(sortedNeighbors.Count))
                    break;

                d = Closure(d + 1, sortedNeighbors, ancestorLists);
            }
        }
        long Closure(long d, List<int> sortedNeighbors, Dictionary<int, List<int>> ancestorLists)
        {
            var done = false;

            while (!done)
            {
                done = true;

                foreach(var v in sortedNeighbors)
                {
                    var vBit = sortedNeighbors.Count - 1 - sortedNeighbors.IndexOf(v);
                    foreach (var w in ancestorLists[v].Intersect(sortedNeighbors))
                    {
                        var wBit = sortedNeighbors.Count - 1 - sortedNeighbors.IndexOf(w);
                        if (d.IsBitSet(wBit) && !d.IsBitSet(vBit))
                        {
                            d = d.SetBit(vBit);
                            done = false;
                        }
                    }
                }
            }

            return d;
        }
        Dictionary<int, List<int>> GetAncestorLists(int i, Dictionary<int, List<int>> outNeighbors)
        {
            var ancestorLists = new Dictionary<int, List<int>>(i);

            for (int v = 0; v < i; v++)
                ancestorLists[v] = new List<int>() { v };

            var done = false;
            while (!done)
            {
                done = true;

                for (int v = 0; v < i; v++)
                {
                    foreach (var w in outNeighbors[v])
                    {
                        if (w >= i)
                            continue;

                        var c = ancestorLists[w].Count;
                        ancestorLists[w] = ancestorLists[w].Union(ancestorLists[v]);

                        if (ancestorLists[w].Count > c)
                            done = false;
                    }
                }
            }

            return ancestorLists;
        }
        #endregion

        #region Classic Algorithms
        public EquivalenceRelation<int> FindFactors()
        {
            var equivalenceRelation = new Algorithms.Utility.EquivalenceRelation<int>();

            for (int i = 0; i < N; i++)
            {
                equivalenceRelation.AddElement(i);

                for (int j = i + 1; j < N; j++)
                {
                    if (!_adjacent[i, j])
                        equivalenceRelation.Relate(i, j);
                }
            }

            return equivalenceRelation;
        }
        public EquivalenceRelation<int> FindComponents()
        {
            var equivalenceRelation = new Algorithms.Utility.EquivalenceRelation<int>();

            for (int i = 0; i < N; i++)
            {
                equivalenceRelation.AddElement(i);

                for (int j = i + 1; j < N; j++)
                {
                    if (_adjacent[i, j])
                        equivalenceRelation.Relate(i, j);
                }
            }

            return equivalenceRelation;
        }
        public EquivalenceRelation<int> FindComponents(List<int> v)
        {
            var equivalenceRelation = new Algorithms.Utility.EquivalenceRelation<int>();

            for (int i = 0; i < v.Count; i++)
            {
                equivalenceRelation.AddElement(v[i]);

                for (int j = i + 1; j < v.Count; j++)
                {
                    if (_adjacent[v[i], v[j]])
                        equivalenceRelation.Relate(v[i], v[j]);
                }
            }

            return equivalenceRelation;
        }
        //public List<List<int>> FindStrongComponentsTarjan()
        //{
        //    var strongComponents = new List<List<int>>();
            
        //    var indices = Enumerable.Repeat(-1, N).ToArray();
        //    var lowlink = Enumerable.Repeat(-1, N).ToArray();
        //    var S = new Stack<int>();
        //    var index = 0;

        //    foreach (var v in _vertices)
        //    {
        //        if (indices[v] < 0)
        //            StrongConnect(v, ref index, indices, lowlink, S, strongComponents);
        //    }

        //    return strongComponents;
        //}
        //void StrongConnect(int v, ref int index, int[] indices, int[] lowlink, Stack<int> S, List<List<int>> strongComponents)
        //{
        //    indices[v] = index;
        //    lowlink[v] = index;
        //    index++;
        //    S.Push(v);

        //    foreach (var w in OutNeighbors[v])
        //    {
        //        if (indices[w] < 0)
        //        {
        //            StrongConnect(w, ref index, indices, lowlink, S, strongComponents);
        //            lowlink[v] = Math.Min(lowlink[v], lowlink[w]);
        //        }
        //        else if (S.Contains(w))
        //        {
        //            lowlink[v] = Math.Min(lowlink[v], indices[w]);
        //        }
        //    }

        //    if (lowlink[v] == indices[v])
        //    {
        //        var component = new List<int>();
                
        //        while (true)
        //        {
        //            var w = S.Pop();
        //            component.Add(w);

        //            if (w == v)
        //                break;
        //        }

        //        strongComponents.Add(component);
        //    }
        //}

        public int ComputeDiameter()
        {
            int[,] distance;
            int[,] next;
            FloydWarshall(out distance, out next);
            int diameter = 0;
            foreach (var v in Vertices)
            {
                foreach (var w in Vertices)
                {
                    if (distance[v, w] > diameter)
                        diameter = distance[v, w];
                }
            }

            return diameter;
        }

        public void FloydWarshall(out int[,] distance, out int[,] next)
        {
            distance = new int[N, N];
            next = new int[N, N];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    distance[i, j] = int.MaxValue / 3;

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i == j)
                        distance[i, j] = 0;
                    else if (_adjacent[i, j])
                    {
                        /*if (Directed[i, j])
                            distance[i, j] = 1;

                        if (Directed[j, i])
                            distance[j, i] = 1;*/

                        distance[i, j] = 1;
                        distance[j, i] = 1;
                    }
                }
            }

            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    next[i, j] = -1;

            for (int k = 0; k < N; k++)
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            next[i, j] = k;
                        }
                    }
                }
            }
        }
        public IEnumerable<int> FloydWarshallShortestPath(int v, int w, int[,] distance, int[,] next)
        {
            if (distance[v, w] == int.MaxValue)
                return null;

            if (next[v, w] == -1)
                return new[] { v, w };

            return FloydWarshallShortestPath(v, next[v, w], distance, next).Union(FloydWarshallShortestPath(next[v, w], w, distance, next).Skip(1));
        }
        //public List<int> FindShortestCycle()
        //{
        //    var shortestLength = int.MaxValue;
        //    var shortestCycle = new List<int>();

        //    foreach (var v in _vertices)
        //    {
        //        var distance = new int[N];
        //        var parent = Enumerable.Repeat(-1, N).ToArray();

        //        var visited = new List<int>();
        //        var leaves = new System.Collections.Generic.Queue<int>();
        //        leaves.Enqueue(v);

        //        while (leaves.Count > 0)
        //        {
        //            var x = leaves.Dequeue();
        //            visited.Add(x);

        //            foreach (var y in Neighbors[x])
        //            {
        //                if (y == parent[x])
        //                    continue;

        //                if (!visited.Contains(y))
        //                {
        //                    parent[y] = x;
        //                    distance[y] = distance[x] + 1;
        //                    leaves.Enqueue(y);
        //                }
        //                else if (distance[x] + distance[y] + 1 < shortestLength)
        //                {
        //                    shortestLength = distance[x] + distance[y] + 1;
        //                    shortestCycle.Clear();

        //                    var w = x;
        //                    while (w != -1)
        //                    {
        //                        shortestCycle.Add(w);
        //                        w = parent[w];
        //                    }

        //                    if (y != v)
        //                    {
        //                        w = y;
        //                        while (w != -1)
        //                        {
        //                            shortestCycle.Insert(0, w);
        //                            w = parent[w];
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return shortestCycle;
        //}
        #endregion
    }
}
