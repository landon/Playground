using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.Utility
{
    public class EquivalenceRelation<T>
    {
        Dictionary<T, Node> _nodeLookup = new Dictionary<T, Node>();

        public void AddElement(T t)
        {
            LookupNode(t);
        }

        public void Relate(T x, T y)
        {
            var nx = LookupNode(x);
            var ny = LookupNode(y);

            var xRoot = Find(nx);
            var yRoot = Find(ny);

            if (xRoot == yRoot)
                return;

            if (xRoot.Rank < yRoot.Rank)
                xRoot.Parent = yRoot;
            else if (xRoot.Rank > yRoot.Rank)
                yRoot.Parent = xRoot;
            else
            {
                yRoot.Parent = xRoot;
                xRoot.Rank++;
            }
        }

        public bool AreRelated(T x, T y)
        {
            var nx = LookupNode(x);
            var ny = LookupNode(y);

            return Find(nx) == Find(ny);
        }

        public IEnumerable<T> GetEquivalenceClass(T x)
        {
            var nx = LookupNode(x);
            var xRoot = Find(nx);

            return _nodeLookup.Values.Where(node => Find(node) == xRoot).Select(node => node.T);
        }

        public IEnumerable<IEnumerable<T>> GetEquivalenceClasses()
        {
            return _nodeLookup.Values.GroupBy(node => Find(node)).Select(group => group.Select(node => node.T));
        }

        Node Find(Node n)
        {
            if (n.Parent != n)
                n.Parent = Find(n.Parent);

            return n.Parent;
        }

        Node LookupNode(T t)
        {
            Node node;
            if (!_nodeLookup.TryGetValue(t, out node))
            {
                node = new Node();
                node.Parent = node;
                node.Rank = 0;
                node.T = t;

                _nodeLookup[t] = node;
            }

            return node;
        }

        class Node
        {
            public Node Parent { get; set; }
            public int Rank { get; set; }
            public T T { get; set; }
        }
    }
}
