using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class DynamicAnalyzer<TList> : IDynamicAnalyzer<TList>
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

        public bool Analyze(IAssignment<TList> assignment, HashSet<IAssignment<TList>> targets)
        {
            throw new NotImplementedException();
        }
    }
}
