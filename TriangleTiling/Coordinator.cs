using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleTiling
{
    class Coordinator
    {
        const string Letters = "ABCDEF";

        public double TileWidth { get; private set; }
        public double TileHeight { get; private set; }

        public Coordinator(double tileWidth, double tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        public List<Tuple<double, double>> LabelToSpaceCoordinates(string l)
        {
            return AbstractToSpaceCoordinates(LabelToAbstractCoordinates(l));
        }

        public string SpaceCoordinatesToLabel(List<Tuple<double, double>> cs)
        {
            return AbstractCoordinatesToLabel(SpaceToAbstractCoordinates(cs));
        }

        public Tuple<int, int, bool> LabelToAbstractCoordinates(string l)
        {
            var n = 0;
            if (string.IsNullOrEmpty(l) || l.Length < 2 || !Letters.Contains(l[0]) || !int.TryParse(l.Substring(1), out n))
                throw new Exception("illegal triangle label");

            var row = Letters.IndexOf(l[0]);
            var column = (n - 1) / 2;
            var leftMost = (n & 1) == 1;

            return new Tuple<int, int, bool>(row, column, leftMost);
        }

        public string AbstractCoordinatesToLabel(Tuple<int, int, bool> a)
        {
            return Letters[a.Item1].ToString() + (2 * a.Item2 + (a.Item3 ? 1 : 2));
        }

        public List<Tuple<double, double>> AbstractToSpaceCoordinates(Tuple<int, int, bool> a)
        {
            return AbstractToSpaceCoordinates(a.Item1, a.Item2, a.Item3);
        }

        public List<Tuple<double, double>> AbstractToSpaceCoordinates(int row, int column, bool leftMost)
        {
            var top = row * TileHeight;
            var left = column * TileWidth;
            var bottom = top + TileHeight;
            var right = left + TileWidth;

            var common = new List<Tuple<double, double>>() { new Tuple<double, double>(left, top), new Tuple<double, double>(right, bottom) };
            if (leftMost)
                common.Add(new Tuple<double, double>(left, bottom));
            else
                common.Add(new Tuple<double, double>(right, top));

            return common;
        }

        public Tuple<int, int, bool> SpaceToAbstractCoordinates(List<Tuple<double, double>> triangleCoordinates)
        {
            var row = triangleCoordinates.Min(c => (int)(c.Item2 / TileHeight));
            var column = triangleCoordinates.Min(c => (int)(c.Item1 / TileWidth));
            var leftMost = triangleCoordinates.Count(c => column == (int)(c.Item1 / TileWidth)) == 2;

            return new Tuple<int, int, bool>(row, column, leftMost);
        }
    }
}
