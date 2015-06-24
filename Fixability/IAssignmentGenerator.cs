using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IAssignmentGenerator
    {
        List<IAssignment> Generate(int[] sizes, int minPot, int maxPot);
    }
}
