using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public interface IGraphics
    {
        void Clear(ARGB argb);
        void DrawLine(ARGB argb, Box p1, Box p2, double width = 1);
        void DrawLine(ARGB argb, double x1, double y1, double x2, double y2, double width = 1);
        void DrawLines(ARGB argb, IEnumerable<Box> points, double width = 1);
        void FillPolygon(ARGB argb, IEnumerable<Box> points);
        void DrawEllipse(ARGB argb, Box bounds, double width = 1);
        void FillEllipse(ARGB color, Box bounds);
        void DrawString(string s, Font font, ARGB argb, Box bounds);
        Box MeasureString(string s, Font font);
    }
}
