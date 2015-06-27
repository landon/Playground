using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability.Basic
{
    public class AssignmentGenerator : IAssignmentGenerator<List<int>, List<int>>
    {
        public List<IAssignment<List<int>, List<int>>> Generate(int[] sizes, int minPot, int maxPot)
        {
            throw new NotImplementedException();
        }
    }
}
