using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    public interface IGraph<TColorSet, TVertexSet>
    {
        int N { get; }
        List<TVertexSet> VertexSubsets { get; }
        IOperations<TColorSet, TVertexSet> Operations { get; }

        bool AreAdjacent(int v, int w);
        void Initialize(bool[,] adjacent);
        bool IsChoosable(Func<int, TColorSet> geTColorSet);
        bool IsChoosableWithoutVertex(int v, Func<int, TColorSet> geTColorSet);
        int EdgeCountIn(TVertexSet set);
    }
}
