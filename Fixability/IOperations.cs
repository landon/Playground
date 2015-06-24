using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IOperations<TColorSet, TVertexSet>
    {
        int Count(TVertexSet set);
        TVertexSet Subset(TVertexSet set, List<int> indices);
    }
}
