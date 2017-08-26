using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouchTest
{
    class Set<T> :IEnumerable<T>
        where T: Item
    {
        T[] _items;
        public Set(int capacity = 256)
        {
            _items = new T[capacity];
        }

        public void Add(T i)
        {
            var j = -1;
            while (++j >= 0)
            {
                if (_items[j] == null || _items[j].Dead)
                {
                    _items[j] = i;
                    break;
                }
            }
        }
        public void Remove(int id)
        {
            var j = -1;
            while (++j >= 0)
            {
                if (_items[j] == null)
                {
                    while (--j > 0 && _items[j].Dead)
                        _items[j] = null;
                    break;
                }
                if (_items[j].Id == id)
                {
                    _items[j].Dead = true;
                    if (_items[++j] == null)
                    {
                        while (--j > 0 && _items[j].Dead)
                            _items[j] = null;
                    }
                    break;
                }
            }
        }
        public bool Contains(int id)
        {
            return Find(id) != null;
        }
        public T Find(int id)
        {
            var j = -1;
            while (++j >= 0)
            {
                if (_items[j] == null)
                {
                    while (--j > 0 && _items[j].Dead)
                        _items[j] = null;
                    break;
                }
                if (_items[j].Id == id)
                {
                    var q = _items[j];
                    if (j > 0 && _items[j - 1].Dead)
                    {
                        var f = _items[j + 1] == null;
                        if (f)
                            _items[j] = null;
                        else
                            _items[j].Dead = true;
                        while (--j > 0 && _items[j].Dead)
                        {
                            if (f)
                                _items[j] = null;
                            else
                                _items[j].Dead = true;
                        }
                        _items[j + 1] = q;
                        _items[j + 1].Dead = false;
                    }
                    return q;
                }
            }
            return null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var j = -1;
            while (++j >= 0)
            {
                if (_items[j] == null)
                    yield break;
                if (!_items[j].Dead)
                    yield return _items[j];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
