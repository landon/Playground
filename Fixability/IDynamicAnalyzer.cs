using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IDynamicAnalyzer<TList>
    {
        bool Analyze(IAssignment<TList> assignment, HashSet<IAssignment<TList>> targets);
        void Initialize(IGraph<TList> graph, IGraph<TList> lineGraph, List<Tuple<int, int>> edges);
    }
}
