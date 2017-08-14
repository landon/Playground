using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class GraphKnowledge
    {
        public Graph Graph { get; private set; }
        public Graph LineGraph { get; private set; }
        public List<Tuple<int, int>> Edges { get; private set; }
        public List<int> EdgeIndices { get; private set; }
        public List<int> Leaves { get; private set; }
        public Lazy<Dictionary<Tuple<int, int>, List<List<int>>>> ConnectedOrderings { get; private set; }

        public GraphKnowledge(Graph g)
        {
            Graph = g;
            Leaves = Graph.Vertices.Where(v => Graph.Degree(v) == 1).ToList();
            BuildLineGraph();

            ConnectedOrderings = new Lazy<Dictionary<Tuple<int, int>, List<List<int>>>>(() =>
            {
                var co = new Dictionary<Tuple<int, int>, List<List<int>>>();
                foreach (var e in Edges)
                    co[e] = Graph.EnumerateConnectedOrderings(new List<int>() { e.Item1, e.Item2 }).ToList();

                return co;
            });
        }

        void BuildLineGraph()
        {
            var adjacent = Graph.Adjacent;
            int n = adjacent.GetUpperBound(0) + 1;

            Edges = new List<Tuple<int, int>>();
            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    if (adjacent[i, j])
                        Edges.Add(new Tuple<int, int>(i, j));

            EdgeIndices = Enumerable.Range(0, Edges.Count).ToList();

            var meets = new bool[Edges.Count, Edges.Count];
            for (int i = 0; i < Edges.Count; i++)
                for (int j = i + 1; j < Edges.Count; j++)
                    if (Edges[i].Item1 == Edges[j].Item1 ||
                        Edges[i].Item1 == Edges[j].Item2 ||
                        Edges[i].Item2 == Edges[j].Item1 ||
                        Edges[i].Item2 == Edges[j].Item2)
                        meets[i, j] = meets[j, i] = true;

            LineGraph = new Graph(meets);
        }
    }
}
