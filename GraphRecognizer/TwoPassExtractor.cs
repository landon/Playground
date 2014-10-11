using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer
{
    public class TwoPassExtractor : IObjectExtractor
    {
        IObjectClassifier Classifier { get; set; }
        public TwoPassExtractor(IObjectClassifier classifier)
        {
            Classifier = classifier;
        }

        public IEnumerable<IObject> EnumerateObjects(IBitGrid grid)
        {
            return ExtractRawCoordinates(grid).Select(kvp => Classifier.ClassifyAndCreate(kvp.Value));
        }

        Dictionary<int, List<Coordinate>> ExtractRawCoordinates(IBitGrid grid)
        {
            var labeling = new int[grid.Width, grid.Height];
            var labelRelation = new EquivalenceRelation<int>();

            var nextLabel = 1;
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (!grid.IsSet(x, y))
                        continue;

                    if (grid.IsSet(x, y - 1))
                    {
                        labeling[x, y] = labeling[x, y - 1];
                    }
                    else if (grid.IsSet(x - 1, y))
                    {
                        labeling[x, y] = labeling[x - 1, y];

                        if (grid.IsSet(x + 1, y - 1))
                        {
                            labelRelation.Relate(labeling[x - 1, y], labeling[x + 1, y - 1]);
                        }
                    }
                    else if (grid.IsSet(x - 1, y - 1))
                    {
                        labeling[x, y] = labeling[x - 1, y - 1];

                        if (grid.IsSet(x + 1, y - 1))
                        {
                            labelRelation.Relate(labeling[x - 1, y - 1], labeling[x + 1, y - 1]);
                        }
                    }
                    else if (grid.IsSet(x + 1, y - 1))
                    {
                        labeling[x, y] = labeling[x + 1, y - 1];
                    }
                    else
                    {
                        labeling[x, y] = nextLabel;
                        labelRelation.AddElement(nextLabel);

                        nextLabel++;
                    }
                }
            }

            var coordinateLabeling = new Dictionary<int, List<Coordinate>>();
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (labeling[x, y] == 0)
                        continue;

                    var objectLabel = labelRelation.FindRepresentative(labeling[x, y]);

                    List<Coordinate> coordinates;
                    if (!coordinateLabeling.TryGetValue(objectLabel, out coordinates))
                    {
                        coordinates = new List<Coordinate>();
                        coordinateLabeling[objectLabel] = coordinates;
                    }

                    coordinates.Add(new Coordinate(x, y));
                }
            }

            return coordinateLabeling;
        }
    }
}
