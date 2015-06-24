using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fixability
{
    public interface IAssignment<TList>
    {
        TList CommonColors(int v, int w);
    }
}
