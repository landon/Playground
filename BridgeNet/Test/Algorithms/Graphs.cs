using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public static class Graphs
    {
        public static Graph E(int n)
        {
            return new Graph(new bool[n, n]);
        }
        public static Graph K(int n)
        {
            return E(n).Complement();
        }
        public static Graph P(int n)
        {
            var G = E(n);
            for (int i = 0; i < n - 1; i++)
                G = G.AddEdge(i, i + 1);

            return G;
        }
        public static Graph Star(int n)
        {
            var G = E(n);
            for (int i = 1; i < n; i++)
                G = G.AddEdge(0, i);

            return G;
        }
        public static Graph C(int n)
        {
            return P(n).AddEdge(n - 1, 0);
        }
        public static Graph Diamond
        {
            get { return E(2) * K(2); }
        }
        public static Graph Bull
        {
            get { return K(3).AttachNewVertex(1).AttachNewVertex(2); }
        }
        public static Graph Kite
        {
            get { return Diamond.AttachNewVertex(1); }
        }
        public static Graph Dart
        {
            get { return Diamond.AttachNewVertex(2); }
        }
        public static Graph Chair
        {
            get { return P(4).AttachNewVertex(2); }
        }
        public static Graph Empty
        {
            get { return new Graph(new bool[0, 0]); }
        }
        public static Graph Paw
        {
            get { return P(4).AddEdge(0, 2); }
        }
        public static Graph H
        {
            get { return P(4).AttachNewVertex(1).AttachNewVertex(2); }
        }
    }
}
