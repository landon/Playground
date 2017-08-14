using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class UnpermutedAssignmentEnumerator : IEnumerable<List<Int64>>
    {
        int _vertexCount;
        Func<int, int> _f;
        int _potSize;
        int _start;
        int _step;

        Dictionary<long, long> _pairMaskToBadMask;

        public UnpermutedAssignmentEnumerator(int vertexCount, Func<int, int> f, int potSize, int start = 0, int step = 1)
        {
            _vertexCount = vertexCount;
            _f = f;
            _potSize = potSize;
            _start = start;
            _step = step;

            _pairMaskToBadMask = Int64Usage.GeneratePairMasks(_potSize);
        }

        public IEnumerator<List<long>> GetEnumerator()
        {
            return GetEnumerable(_vertexCount).Where((list, i) => (i - _start) % _step == 0).GetEnumerator();
        }

        IEnumerable<List<long>> GetEnumerableRecursive(int vertexCount)
        {
            if (vertexCount == 1)
                return (((long)1 << _f(0)) - 1).EnList().EnList();

            return GetEnumerableRecursive(vertexCount - 1).SelectMany(list => GetExtensions(list));
        }

        IEnumerable<List<long>> GetEnumerable(int vertexCount)
        {
            IEnumerable<List<long>> enumerable = (((long)1 << _f(0)) - 1).EnList().EnList();

            for (int i = 1; i < vertexCount; i++)
                enumerable = enumerable.SelectMany(list => GetExtensions(list));

            return enumerable;
        }

        IEnumerable<List<long>> GetExtensions(List<long> list)
        {
            var restricted = _pairMaskToBadMask.Where(kvp => IsRestricted(kvp.Key, list));

            int size = _f(list.Count);

            return from subset in new SubsetEnumerator(_potSize, size)
                   where restricted.All(kvp => (subset & kvp.Key) != kvp.Value)
                   select list.Concat(new[] { subset }).ToList();
        }

        bool IsRestricted(long p, List<long> list)
        {
            foreach (var x in list)
            {
                var intersection = x & p;
                if (intersection != 0 && intersection != x)
                    return false;
            }

            return true;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
