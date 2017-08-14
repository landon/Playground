using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Algorithms.Utility
{
    public class SafeCollection<T> : ICollection<T>
    {
        int _count;
        Node _first;
        Node _last;

        public SafeCollection()
        {
            _first = new Node();
            _last = _first;
        }

        public void Add(T item)
        {
            _last.Next = new Node() { Item = item };
            _last = _last.Next;
            _count++;
        }

        public void Clear()
        {
            _first.Next = null;
            _last = _first;
            _count = 0;
        }

        public bool Contains(T item)
        {
            return FindPreviousNode(item) != null;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var node = _first;
            while (true)
            {
                var next = node.Next;
                if (next == null)
                    break;

                array[arrayIndex++] = next.Item;
                node = next;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            var node = FindPreviousNode(item);
            if (node == null)
                return false;

            var next = node.Next;
            if (next == null)
                return false;
         
            node.Next = next.Next;
            _count--;
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = _first;
            while (true)
            {
                var next = node.Next;
                if (next == null)
                    break;
             
                yield return next.Item;
                node = next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Node FindPreviousNode(T item)
        {
            var node = _first;
            while (true)
            {
                var next = node.Next;
                if (next == null)
                    break;

                if (EqualityComparer<T>.Default.Equals(next.Item, item))
                    return node;

                node = next;
            }

            return null;
        }

        class Node
        {
            public T Item;
            public Node Next;
        }
    }
}
