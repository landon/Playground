using Bridge.Html5;
using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Graphics : IGraphics
    {
        HTMLCanvasElement _canvas;
        CanvasRenderingContext2D _context;

        public Graphics(HTMLCanvasElement canvas)
        {
            _canvas = canvas;
            _context = canvas.GetContext("2d").As<CanvasRenderingContext2D>();
        }

        public void Clear(ARGB argb)
        {
            _context.FillStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.ClearRect(0, 0, _canvas.Width, _canvas.Height);
            _context.FillRect(0, 0, _canvas.Width, _canvas.Height);
        }

        public void DrawLine(ARGB argb, Box p1, Box p2, double width = 1)
        {
            _context.StrokeStyle = argb.ToColor();
            _context.LineWidth = width;
            _context.GlobalAlpha = Math.Min(1.0f, Math.Max(0.0f, argb.A / 255.0f));
            _context.BeginPath();
            _context.MoveTo(p1.X, p1.Y);
            _context.LineTo(p2.X, p2.Y);
            _context.Stroke();
        }
        public void DrawLine(ARGB argb, double x1, double y1, double x2, double y2, double width = 1)
        {
            DrawLine(argb, new Box(x1, y1), new Box(x2, y2), width);
        }
        public void DrawLines(ARGB argb, IEnumerable<Box> points, double width = 1)
        {
            _context.StrokeStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.LineWidth = width;
            _context.BeginPath();
            var first = true;
            foreach (var b in points)
            {
                if (first)
                {
                    _context.MoveTo(b.X, b.Y);
                    first = false;
                }
                else
                {
                    _context.LineTo(b.X, b.Y);
                }
            }
            _context.Stroke();
        }
        public void FillPolygon(ARGB argb, IEnumerable<Box> points)
        {
            _context.FillStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.BeginPath();
            var first = true;
            foreach (var b in points)
            {
                if (first)
                {
                    _context.MoveTo(b.X, b.Y);
                    first = false;
                }
                else
                {
                    _context.LineTo(b.X, b.Y);
                }
            }
            _context.Fill();
        }
        public void DrawEllipse(ARGB argb, Box bounds, double width = 1)
        {
            _context.StrokeStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.BeginPath();
            _context.Arc(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2, bounds.Width / 2, 0, 2 * Math.PI);
            _context.Stroke();
        }
        public void FillEllipse(ARGB argb, Box bounds)
        {
            _context.BeginPath();
            _context.FillStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.Arc(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2, bounds.Width / 2, 0, 2 * Math.PI);
            _context.Fill();
        }
        public void DrawString(string s, Font font, ARGB argb, Box bounds)
        {
            _context.Font = font.Name + " " + font.Size + "px";
            _context.StrokeStyle = argb.ToColor();
            _context.GlobalAlpha = argb.A / 255.0f;
            _context.StrokeText(s, (int)bounds.X, (int)bounds.Y);
        }
        public Box MeasureString(string s, Font font)
        {
            _context.Font = font.Name + " " + font.Size + "px";
            var metrics = _context.MeasureText(s);

            return new Box(0, 0, metrics.Width, metrics.EmHeightAscent + metrics.EmHeightDescent);
        }
    }
}
