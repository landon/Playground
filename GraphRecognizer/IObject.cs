using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphRecognizer
{
    public interface IObject
    {
        IList<Coordinate> Coordinates { get; }
    }
}
