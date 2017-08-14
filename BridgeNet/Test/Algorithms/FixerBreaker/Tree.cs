using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker
{
    public class Tree<T>  where T : Tree<T>
    {
        public T Parent { get; private set; }
        T This { get; set; }
        public List<T> Children { get; private set; }

        public Tree()
        {
            This = (T)this;
            Children = new List<T>();
        }

        public void RemoveChild(T child)
        {
            Children.Remove(child);
        }

        protected void AddChild(T child)
        {
            child.Parent = This;
            Children.Add(child);
        }

        public int GetDepth()
        {
            var depth = 0;
            foreach (var child in Children)
                depth = Math.Max(depth, child.GetDepth() + 1);

            return depth;
        }

        protected void Traverse(Action<T, T> onDescent, int maxDepth = int.MaxValue)
        {
            TraverseInternal(onDescent, maxDepth, 0);
        }

        void TraverseInternal(Action<T, T> onDescent, int maxDepth, int depth)
        {
            if (depth >= maxDepth - 1)
                return;

            foreach (var child in Children)
            {
                onDescent(This, child);
                child.TraverseInternal(onDescent, maxDepth, depth + 1);
            }
        }

        public Tuple<Graph, Dictionary<int, T>> BuildGraph(int maxDepth = int.MaxValue)
        {
            var vertices = 0;
            var vertexLookup = new Dictionary<T, int>();
            var treeLookup = new Dictionary<int, T>();
            var edges = new Dictionary<int, List<int>>();

            Traverse((parent, child) =>
            {
                int p;
                if (!vertexLookup.TryGetValue(parent, out p))
                {
                    p = vertices++;
                    vertexLookup[parent] = p;
                    treeLookup[p] = parent;

                    edges[p] = new List<int>();
                }

                int c;
                if (!vertexLookup.TryGetValue(child, out c))
                {
                    c = vertices++;
                    vertexLookup[child] = c;
                    treeLookup[c] = child;

                    edges[c] = new List<int>();
                }

                edges[p].Add(c);
            }, maxDepth);

            if (vertices == 0)
            {
                treeLookup[0] = (T)this;
                edges[0] = new List<int>();
            }

            var edgeWeights = new List<int>();
            for (int i = 0; i < vertices; i++)
            {
                for (int j = i + 1; j < vertices; j++)
                {
                    if (edges[i].Contains(j))
                        edgeWeights.Add(1);
                    else if (edges[j].Contains(i))
                        edgeWeights.Add(-1);
                    else
                        edgeWeights.Add(0);
                }
            }

            return new Tuple<Graph, Dictionary<int, T>>(new Graph(edgeWeights), treeLookup);
        }
    }
}
