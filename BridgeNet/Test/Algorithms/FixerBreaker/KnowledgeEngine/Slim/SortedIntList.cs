using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim
{
    public class SortedIntList
    {
        int[] _data;
        int _length;

        public SortedIntList(int initialCapacity = 16)
        {
            _data = new int[initialCapacity];
        }

        public void Add(int x)
        {
            var i = Array.BinarySearch<int>(_data, 0, _length, x);
            if (i >= 0)
                return;

            EnsureCapacity();

            i = ~i;
            if (i < _length)
                Array.Copy(_data, i, _data, i + 1, _length - i);

            _data[i] = x;
            _length++;
        }

        public bool Contains(int x)
        {
            return Array.BinarySearch<int>(_data, 0, _length, x) >= 0;
        }

        public bool Intersects(SortedIntList other)
        {
            int j = 0;
            for (int i = 0; i < _length; i++)
            {
                var k = Array.BinarySearch<int>(other._data, j, other._length - j, _data[i]);
                if (k >= 0)
                    return true;
                j = ~k;
            }
           
            return false;
        }

        void EnsureCapacity()
        {
            if (_length >= _data.Length)
            {
                var data = new int[2 * _data.Length];
                Array.Copy(_data, data, _data.Length);
                _data = data;
            }
        }
    }
}
