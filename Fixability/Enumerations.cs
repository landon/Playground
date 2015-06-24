using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixability
{
    [Flags]
    public enum AnalysisResult
    {
        NotFixable = 0,
        FixableForSuperabundantAssignments = 1,
        FixableForNearlyColorableAssignments = 2,
        FixableForNearlyColorableSuperabundantAssignments = 4,
        FixableForAllAssignments = 8
    }
}