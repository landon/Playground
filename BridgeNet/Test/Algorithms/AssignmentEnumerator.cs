using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class AssignmentEnumerator: IEnumerable<List<Int64>>
    {
        int _vertexCount;
        Func<int, int> _f;
        int _potSize;
        int _start;
        int _step;

        public AssignmentEnumerator(int vertexCount, Func<int, int> f, int potSize, int start = 0, int step = 1)
        {
            _vertexCount = vertexCount;
            _f = f;
            _potSize = potSize;
            _start = start;
            _step = step;
        }

        #region IEnumerable<long[]> Members

        public IEnumerator<List<Int64>> GetEnumerator()
        {
            return new Enumerator(_vertexCount, _f, _potSize, _start, _step);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        class Enumerator : IEnumerator<List<Int64>>
        {
            List<IEnumerator<Int64>> _enumerators;
            List<Int64> _current;
            int _step;

            public Enumerator(int vertexCount, Func<int, int> f, int potSize, int start = 0, int step = 1)
            {
                _enumerators = new List<IEnumerator<Int64>>(vertexCount);
                var first = Enumerable.Range(0, f(0)).ToInt64();
                _enumerators.Add(new List<Int64>() { first }.GetEnumerator());
                for (int i = 1; i < vertexCount; i++)
                    _enumerators.Add(new SubsetEnumerator.Enumerator(potSize, f(i)));
             
                _step = step;

                for (int i = 0; i < start; i++) if (!MoveNextInternal()) break;
            }

            #region IEnumerator<long[]> Members

            public List<Int64> Current
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
                for (int j = 0; j < _step; j++) if (!MoveNextInternal()) return false;

                return true;
            }

            bool MoveNextInternal()
            {
                if (_current == null)
                {
                    _current = new List<Int64>(_enumerators.Count);

                    foreach(var enumerator in _enumerators)
                    {
                        enumerator.MoveNext();
                        _current.Add(enumerator.Current);
                    }

                    return true;
                }

                int i = _enumerators.Count - 1;
                while (i >= 1)
                { 
                    if (_enumerators[i].MoveNext())
                    {
                        _current[i] = _enumerators[i].Current;
                        break;
                    }

                    _enumerators[i].Reset();
                    _enumerators[i].MoveNext();

                    _current[i] = _enumerators[i].Current;

                    i--;
                }

                return i >= 1;
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
