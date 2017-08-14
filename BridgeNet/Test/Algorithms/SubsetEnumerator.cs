using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class SubsetEnumerator : IEnumerable<Int64>
    {
        int _setSize;
        int _subsetSize;

        public SubsetEnumerator(int setSize, int subsetSize)
        {
            _setSize = setSize;
            _subsetSize = subsetSize;
        }

        #region IEnumerable<long> Members

        public IEnumerator<Int64> GetEnumerator()
        {
            return new Enumerator(_setSize, _subsetSize);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public class Enumerator : IEnumerator<Int64>
        {
            bool _isFirst = true;
            int _setSize;
            int _subsetSize;

            Int64 _current;

            public Enumerator(int setSize, int subsetSize)
            {
                _setSize = setSize;
                _subsetSize = subsetSize;
            }

            #region IEnumerator<long> Members

            public long Current
            {
                get { return _current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (_isFirst)
                {
                    if (_subsetSize > _setSize) return false;

                    _current = ((1 << _subsetSize) - 1) << (_setSize - _subsetSize);

                    _isFirst = false;
                    return true;
                }

                Int64 lsb = _current & -_current;

                if (lsb > 1)
                {
                    _current = _current ^ lsb ^ (lsb >> 1);

                    return true;
                }

                Int64 high = _current & (_current + 1);

                if (high == 0) return false;

                Int64 low = _current ^ high;
                low = (low << 2) + 3;

                while ((low & high) == 0) low <<= 1;

                _current = high ^ low;

                return true;
            }

            public void Reset()
            {
                _isFirst = true;
            }

            #endregion
        }
    }
}
