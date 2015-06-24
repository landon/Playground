using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IStaticAnalyzer
    {
        bool IsColorable(IAssignment assignment);
        bool IsSuperabundant(IAssignment assignment);
        bool IsNearlyColorable(IAssignment assignment);
    }
}
