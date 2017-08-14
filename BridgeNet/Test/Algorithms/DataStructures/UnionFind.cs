using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.DataStructures
{
    public class UnionFind<T>
    {
        public Node MakeSet(T t)
        {
            var node = new Node();
            node.Parent = node;
            node.Rank = 0;
            node.T = t;

            RootAdded(node);
            return node;
        }

        public void Union(Node nx, Node ny)
        {
            var xRoot = Find(nx);
            var yRoot = Find(ny);

            if (xRoot == yRoot)
                return;

            if (xRoot.Rank < yRoot.Rank)
            {
                xRoot.Parent = yRoot;
                RootRemoved(xRoot);
            }
            else if (xRoot.Rank > yRoot.Rank)
            {
                yRoot.Parent = xRoot;
                RootRemoved(yRoot);
            }
            else
            {
                yRoot.Parent = xRoot;
                xRoot.Rank++;
                RootRemoved(yRoot);
            }
        }

        public Node Find(Node n)
        {
            if (n.Parent != n)
                n.Parent = Find(n.Parent);

            return n.Parent;
        }

        public class Node
        {
            public Node Parent { get; set; }
            public int Rank { get; set; }
            public T T { get; set; }
        }

        protected virtual void RootAdded(Node n) { }
        protected virtual void RootRemoved(Node n) { }
    }
}
