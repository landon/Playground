using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphRecognizer
{
    public class BitmapGrid : IBitGrid
    {
        Bitmap Bitmap { get; set; }

        public int Width { get { return Bitmap.Width; } }
        public int Height { get { return Bitmap.Height; } }

        public BitmapGrid(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public bool IsSet(int x, int y)
        {
            var color = Bitmap.GetPixel(x, y);
            return (color.R + color.G + color.B) / 3 < 127;
        }
    }
}
