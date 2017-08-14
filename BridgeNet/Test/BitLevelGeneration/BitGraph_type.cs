
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BitLevelGeneration
{
    public class BitGraph_long : IGraph_long
    {
        int[] _vertices;
        long[] _neighborhood;

        public BitGraph_long(List<int> edgeWeights)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * edgeWeights.Count)) / 2);
            _vertices = Enumerable.Range(0, n).ToArray();

            _neighborhood = new long[n];

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                var iBit = 1L << i;
                for (int j = i + 1; j < n; j++)
                {
                    var jBit = 1L << j;

                    if (edgeWeights[k] != 0)
                    {
                        _neighborhood[i] |= jBit;
                        _neighborhood[j] |= iBit;
                    }

                    k++;
                }
            }
        }

		public BitGraph_long(int n, long[] neighborhood)
        {
			_vertices = Enumerable.Range(0, n).ToArray();
			_neighborhood = neighborhood;
		}

        public int N { get { return _vertices.Length; } }
        public IEnumerable<int> Vertices { get { return _vertices; } }

        public bool IsIndependent(long set)
        {
            return set.TrueForAllBitIndices(i => (_neighborhood[i] & set) == 0);
        }

        public int Degree(int v)
        {
            return _neighborhood[v].PopulationCount();
        }
        public int DegreeInSet(int v, long set)
        {
            return (_neighborhood[v] & set).PopulationCount();
        }
		public long NeighborsInSet(int v, long set)
        {
            return _neighborhood[v] & set;
        }
        public IEnumerable<long> MaximalIndependentSubsets(long set)
        {
            var list = new List<long>(8);
            BronKerbosch(set, 0L, 0L, list);
            return list;
        }

        void BronKerbosch(long P, long R, long X, List<long> list)
        {
            if (P == 0 && X == 0)
                list.Add(R);
            else
            {
                var u = TomitaPivot(P, X);
                var q = P & ((1L << u) | _neighborhood[u]);

                while (q != 0)
                {
                    var bit = q & -q;
                    var v = bit.Extract();
                    var non = ~(bit | _neighborhood[v]);

                    BronKerbosch(P & non, R | bit, X & non, list);

                    q ^= bit;
                    P ^= bit;
                    X |= bit;
                }
            }
        }

        int TomitaPivot(long P, long X)
        {
            var min = int.MaxValue;
            var best = -1;
            var q = P | X;

            while (q != 0)
            {
                var bit = q & -q;
                var u = bit.Extract();
				
				var n = ((bit | _neighborhood[u]) & P).PopulationCount();
                if (n < min)
                {
                    min = n;
                    best = u;
                }

                q ^= bit;
            }

            return best;
        }
    }
}


namespace BitLevelGeneration
{
    public class BitGraph_uint : IGraph_uint
    {
        int[] _vertices;
        uint[] _neighborhood;

        public BitGraph_uint(List<int> edgeWeights)
        {
            var n = (int)((1 + Math.Sqrt(1 + 8 * edgeWeights.Count)) / 2);
            _vertices = Enumerable.Range(0, n).ToArray();

            _neighborhood = new uint[n];

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                var iBit = 1U << i;
                for (int j = i + 1; j < n; j++)
                {
                    var jBit = 1U << j;

                    if (edgeWeights[k] != 0)
                    {
                        _neighborhood[i] |= jBit;
                        _neighborhood[j] |= iBit;
                    }

                    k++;
                }
            }
        }

		public BitGraph_uint(int n, uint[] neighborhood)
        {
			_vertices = Enumerable.Range(0, n).ToArray();
			_neighborhood = neighborhood;
		}

        public int N { get { return _vertices.Length; } }
        public IEnumerable<int> Vertices { get { return _vertices; } }

        public bool IsIndependent(uint set)
        {
            return set.TrueForAllBitIndices(i => (_neighborhood[i] & set) == 0);
        }

        public int Degree(int v)
        {
            return _neighborhood[v].PopulationCount();
        }
        public int DegreeInSet(int v, uint set)
        {
            return (_neighborhood[v] & set).PopulationCount();
        }
		public uint NeighborsInSet(int v, uint set)
        {
            return _neighborhood[v] & set;
        }
        public IEnumerable<uint> MaximalIndependentSubsets(uint set)
        {
            var list = new List<uint>(8);
            BronKerbosch(set, 0U, 0U, list);
            return list;
        }

        void BronKerbosch(uint P, uint R, uint X, List<uint> list)
        {
            if (P == 0 && X == 0)
                list.Add(R);
            else
            {
                var u = TomitaPivot(P, X);
                var q = P & ((1U << u) | _neighborhood[u]);

                while (q != 0)
                {
                    var bit = q & (~q + 1);
                    var v = bit.Extract();
                    var non = ~(bit | _neighborhood[v]);

                    BronKerbosch(P & non, R | bit, X & non, list);

                    q ^= bit;
                    P ^= bit;
                    X |= bit;
                }
            }
        }

        int TomitaPivot(uint P, uint X)
        {
            var min = int.MaxValue;
            var best = -1;
            var q = P | X;

            while (q != 0)
            {
                var bit = q & (~q + 1);
                var u = bit.Extract();
				
				var n = ((bit | _neighborhood[u]) & P).PopulationCount();
                if (n < min)
                {
                    min = n;
                    best = u;
                }

                q ^= bit;
            }

            return best;
        }
    }
}


