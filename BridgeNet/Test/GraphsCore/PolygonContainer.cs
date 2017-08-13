using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GraphicsLayer;

namespace Graphs
{
    public class PolygonContainer
    {
        IList<Box> _dots;
        Box _boundingBox;

        public PolygonContainer(IList<Box> dots)
        {
            _dots = dots;

            var minx = _dots.Min(dot => dot.X);
            var maxx = _dots.Max(dot => dot.X);
            var miny = _dots.Min(dot => dot.Y);
            var maxy = _dots.Max(dot => dot.Y);

            _boundingBox = new Box(minx, miny, maxx - minx, maxy - miny);
        }

        public bool Contains(Box dot)
        {
            if (!_boundingBox.Contains(dot))
                return false;

            var contains = false;
            var j = _dots.Count - 1;

            for (var i = 0; i < _dots.Count; i++)
            {
                if ((_dots[i].Y <= dot.Y && dot.Y < _dots[j].Y || _dots[j].Y <= dot.Y && dot.Y < _dots[i].Y) &&  dot.X < (_dots[j].X - _dots[i].X) * (dot.Y - _dots[i].Y) / (_dots[j].Y - _dots[i].Y) + _dots[i].X)
                    contains = !contains;

                j = i;
            }

            return contains;
        }
    }

}
