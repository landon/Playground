using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer.Classifiers
{
    public class DumbObjectClassifier : IObjectClassifier
    {
        public IObject ClassifyAndCreate(IList<Coordinate> coordinates)
        {
            return new GenericObject() { Coordinates = coordinates };
        }
    }
}
