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
        List<T> _items = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)(this)).GetEnumerator();
        }

        public void Remove(int i)
        {
            var t = Find(i);
            if (t != null)
                _items.Remove(t);
        }

        public T Find(int i)
        {
            return _items.FirstOrDefault(x => x.Id == i);
        }

        public void Add(T t)
        {
            if (Has(t.Id))
                return;
            _items.Add(t);
        }

        public bool Has(int i)
        {
            return Find(i) != null;
        }
    }
}
