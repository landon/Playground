using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IDynamicAnalyzer
    {
        bool Analyze(IAssignment assignment, HashSet<IAssignment> targets);
    }
}
