using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public class ARGB
    {
        public static ARGB BasicPalette(int q)
        {
            if (q == -1)
                return new ARGB(255, 255, 255);

            return FromHSL(0.6 + (double)ReverseBits((uint)q) / Math.Pow(2.0f, 32), 1.0, 0.65);
        }

        public static ARGB FromFractional(double a, double r, double g, double b)
        {
            return new ARGB((int)(a * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255)); 
        }

        public static ARGB FromHSL(double h, double s, double l)
        {
            while (h > 1.0)
                h -= 1.0;
            while (s > 1.0)
                s -= 1.0;
            while (l > 1.0)
                l -= 1.0;

            if (s == 0.0)
                return FromFractional(1.0, l, l, l);

            var q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
            var p = 2.0 * l - q;
            var r = HueToRgb(p, q, h + 1.0 / 3.0);
            var g = HueToRgb(p, q, h);
            var b = HueToRgb(p, q, h - 1.0 / 3.0);

            return FromFractional(1.0, r, g, b);
        }

        static double HueToRgb(double p, double q, double t)
        {
            if (t < 0.0) t += 1.0f;
            if (t > 1.0) t -= 1.0;
            if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
            if (t < 1.0 / 2.0) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
            return p;
        }

        static uint ReverseBits(uint x)
        {
            uint y = 0;
            for (int i = 0; i < 32; i++)
                y |= (uint)((x & (1 << i)) << (31 - 2 * i));

            return y;
        }

        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public ARGB(int a, int r, int g, int b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
        public ARGB(int r, int g, int b)
            : this(255, r, g, b)
        {
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", A, R, G, B);
        }
    }
}
