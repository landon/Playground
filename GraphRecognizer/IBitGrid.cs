using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer
{
    public interface IBitGrid
    {
        int Width { get; }
        int Height { get; }

        bool IsSet(int x, int y);
    }
}
