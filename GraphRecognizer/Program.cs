using GraphRecognizer.Classifiers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var bitmap = (Bitmap)Bitmap.FromFile(@"C:\Users\landon\Documents\GitHub\Playground\GraphRecognizer\TestImages\test.png");
            var grid = new BitmapGrid(bitmap);

            var extractor = new TwoPassExtractor(new EdgeVertexClassifier());
            var objects = extractor.EnumerateObjects(grid).ToList();

            Console.WriteLine(string.Format("{0} vertices, {1} edges", objects.Count(o => o is Vertex), objects.Count(o => o is Edge)));
            Console.ReadKey();
        }
    }
}
