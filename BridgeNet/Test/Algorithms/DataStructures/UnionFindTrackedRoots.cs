using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.DataStructures
{
    public class UnionFindTrackedRoots<T> : UnionFind<T>
    {
        List<Node> _roots = new List<Node>();

        public IEnumerable<Node> FindAllExcept(IEnumerable<Node> nodes)
        {
            return _roots.Except(nodes.Select(n => Find(n)));
        }

        protected override void RootAdded(Node n)
        {
            _roots.Add(n);
        }

        protected override void RootRemoved(Node n)
        {
            _roots.Remove(n);
        }
    }
}
