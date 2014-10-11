using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphRecognizer
{
    public interface IObjectClassifier
    {
        IObject ClassifyAndCreate(IList<Coordinate> coordinates);
    }
}
