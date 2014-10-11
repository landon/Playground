using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer.Classifiers
{
    public class EdgeVertexClassifier : IObjectClassifier
    {
        public IObject ClassifyAndCreate(IList<Coordinate> coordinates)
        {
            // TODO: classify as edge or vertex based on shape
            return new Vertex();
        }
    }
}
