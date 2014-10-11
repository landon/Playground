using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer.Classifiers
{
    public class GenericObject : IObject
    {
        public IList<Coordinate> Coordinates
        {
            get;
            set;
        }
    }
}
