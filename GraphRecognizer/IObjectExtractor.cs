using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer
{
    public interface IObjectExtractor
    {
        IEnumerable<IObject> EnumerateObjects(IBitGrid grid);
    }
}
