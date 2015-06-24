using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IAssignment<TColorSet, TVertexSet>
    {
        TColorSet CommonColors(int v, int w);
        int Psi(TVertexSet set);
        int ColorCount { get; }
        TVertexSet GetSwappable(int alpha, int beta);
        IAssignment<TColorSet, TVertexSet> PerformSwap(int alpha, int beta, TVertexSet swapVertices);
    }
}
