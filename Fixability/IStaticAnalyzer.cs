using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IStaticAnalyzer<TList>
    {
        bool IsEdgeColorable(IAssignment<TList> assignment);
        bool IsSuperabundant(IAssignment<TList> assignment);
        bool IsNearlyEdgeColorable(IAssignment<TList> assignment);
        void Initialize(IGraph<TList> graph, IGraph<TList> lineGraph, List<Tuple<int, int>> edges);
    }
}
