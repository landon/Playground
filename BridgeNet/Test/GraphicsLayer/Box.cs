using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public class Box
    {
        public static readonly Box Empty = new Box(0, 0);

        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public double X { get { return Left; } }
        public double Y { get { return Top; } }
        public double Right { get { return Left + Width; } }
        public double Bottom { get { return Top + Height; } }

        public Box(double left, double top)
        {
            Left = left;
            Top = top;
        }
        public Box(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public void Inflate(double x, double y)
        {
            Left -= x;
            Top -= y;
            Width += 2 * x;
            Height += 2 * y;
        }
        public bool Contains(double x, double y)
        {
            return Left <= x && x <= Right &&
                   Top <= y && y <= Bottom;
        }
        public bool Contains(Box box)
        {
            return Left <= box.Left && box.Right <= Right &&
                   Top <= box.Top && box.Bottom <= Bottom;
        }
    }
}
