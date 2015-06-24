using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IAssignmentGenerator<TColorSet, TVertexSet>
    {
        List<IAssignment<TColorSet, TVertexSet>> Generate(int[] sizes, int minPot, int maxPot);
    }
}
