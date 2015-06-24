using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public class StaticAnalyzer<TColorSet, TVertexSet>
    {
        IGraph<TColorSet, TVertexSet> _graph;
        IGraph<TColorSet, TVertexSet> _lineGraph;
        List<Tuple<int, int>> _edges;

        public void Initialize(IGraph<TColorSet, TVertexSet> graph, IGraph<TColorSet, TVertexSet> lineGraph, List<Tuple<int, int>> edges)
        {
            _graph = graph;
            _lineGraph = lineGraph;
            _edges = edges;
        }

        public bool IsEdgeColorable(IAssignment<TColorSet, TVertexSet> assignment)
        {
            return _lineGraph.IsChoosable(e => assignment.CommonColors(_edges[e].Item1, _edges[e].Item2));
        }

        public bool IsSuperabundant(IAssignment<TColorSet, TVertexSet> assignment)
        {
            return _graph.VertexSubsets.All(set => assignment.Psi(set) >= _graph.EdgeCountIn(set));
        }

        public bool IsNearlyEdgeColorable(IAssignment<TColorSet, TVertexSet> assignment)
        {
            return Enumerable.Range(0, _lineGraph.N).Any(e => IsNearlyColorableForEdge(assignment, e));
        }

        public bool IsNearlyColorableForEdge(IAssignment<TColorSet, TVertexSet> assignment, int edgeIndex)
        {
            return _lineGraph.IsChoosableWithoutVertex(edgeIndex, e => assignment.CommonColors(_edges[e].Item1, _edges[e].Item2));
        }
    }
}
