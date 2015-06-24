using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class StaticAnalyzer<TList> : IStaticAnalyzer<TList>
    {
        IGraph<TList> _graph;
        IGraph<TList> _lineGraph;
        List<Tuple<int, int>> _edges;

        public void Initialize(IGraph<TList> graph, IGraph<TList> lineGraph, List<Tuple<int, int>> edges)
        {
            _graph = graph;
            _lineGraph = lineGraph;
            _edges = edges;
        }

        public bool IsEdgeColorable(IAssignment<TList> assignment)
        {
            return _lineGraph.IsChoosable(e => assignment.CommonColors(_edges[e].Item1, _edges[e].Item2));
        }

        public bool IsSuperabundant(IAssignment<TList> assignment)
        {
            throw new NotImplementedException();
        }

        public bool IsNearlyEdgeColorable(IAssignment<TList> assignment)
        {
            throw new NotImplementedException();
        }
    }
}
