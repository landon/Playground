using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class Assignment<TColorSet, TVertexSet> : IAssignment<TColorSet, TVertexSet>
    {
        public TColorSet CommonColors(int v, int w)
        {
            throw new NotImplementedException();
        }

        public int Psi(TVertexSet set)
        {
            throw new NotImplementedException();
        }

        public int ColorCount
        {
            get { throw new NotImplementedException(); }
        }

        public TVertexSet GetSwappable(int alpha, int beta)
        {
            throw new NotImplementedException();
        }

        public IAssignment<TColorSet, TVertexSet> PerformSwap(int alpha, int beta, TVertexSet swapVertices)
        {
            throw new NotImplementedException();
        }
    }
}
