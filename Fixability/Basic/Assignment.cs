using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class Assignment : IAssignment<List<int>, List<int>>
    {
        List<List<int>> _lists;
        List<List<int>> _dual;
        int _potSize;
        int _hashCode;

        public int PotSize { get { return _potSize; } }

        public Assignment(List<List<int>> lists, int potSize)
        {
            _lists = lists;
            _potSize = potSize;

            ComputeDual();
            ComputeHashCode();
        }

        void ComputeDual()
        {
            _dual = new List<List<int>>(_potSize);
            for (int i = 0; i < _potSize; i++)
            {
                _dual[i] = new List<int>();
                for (int j = 0; j < _lists.Count; j++)
                {
                    if (_lists[j].Contains(i))
                        _dual[i].Add(j);
                }
            }

            _dual.Sort(LexOrdering);
        }

        int LexOrdering(List<int> a, List<int> b)
        {
            for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            {
                if (a[i] < b[i])
                    return -1;
                if (a[i] > b[i])
                    return 1;
            }

            if (a.Count < b.Count)
                return -1;
            if (a.Count > b.Count)
                return 1;
            
            return 0;
        }

        void ComputeHashCode()
        {
            _hashCode = 239;
            unchecked
            {
                foreach (var l in _dual)
                {
                    var bits = 0;
                    for (int j = 0; j < l.Count; j++)
                        bits |= (1 << (j % 29));

                    _hashCode = _hashCode * 37 + bits;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals((Assignment)obj);
        }

        public bool Equals(Assignment other)
        {
            if (other._dual.Count != _dual.Count)
                return false;

            for (int i = 0; i <_dual.Count; i++)
            {
                if (!_dual[i].SequenceEqual(other._dual[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public List<int> CommonColors(int v, int w)
        {
            return _lists[v].Intersect(_lists[v]).ToList();
        }

        public int Psi(List<int> set)
        {
            var psi = 0;

            for (int i = 0; i < _potSize; i++)
            {
                var count = _lists.Where((ll, j) => set.Contains(j) && ll.Contains(i)).Count();
                psi += count / 2;
            }

            return psi;
        }

        public List<int> GetSwappable(int alpha, int beta)
        {
            var singleIndices = new List<int>(_lists.Count);
            for (int i = 0; i < _lists.Count; i++)
            {
                var hA = _lists[i].Contains(alpha);
                var hB = _lists[i].Contains(beta);

                if (hA ^ hB)
                    singleIndices.Add(i);
            }

            return singleIndices;
        }

        public IAssignment<List<int>, List<int>> PerformSwap(int alpha, int beta, List<int> swapVertices)
        {
            var swappedLists = new List<List<int>>(_lists.Count);
            for (int i = 0; i < _lists.Count; i++)
            {
                var list = _lists[i].ToList();
                if (swapVertices.Contains(i))
                {
                    if (list.Contains(alpha))
                    {
                        list.Remove(alpha);
                        list.Add(beta);
                    }
                    else
                    {
                        list.Remove(beta);
                        list.Add(alpha);
                    }

                    list.Sort();
                }

                swappedLists.Add(list);
            }

            return new Assignment(swappedLists, _potSize);
        }
    }
}
