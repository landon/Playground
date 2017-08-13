using GraphicsLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class HelperExtensions
    {
        public static string ToColor(this ARGB argb)
        {
            return string.Format("rgba({0},{1},{2},{3:0.00})", argb.R, argb.G, argb.B, argb.A / 255.0);
        }
    }
}
