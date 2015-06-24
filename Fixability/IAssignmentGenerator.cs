using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IAssignmentGenerator<TList>
    {
        List<IAssignment<TList>> Generate(int[] sizes, int minPot, int maxPot);
    }
}
