using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using GraphicsLayer;

namespace Graphs
{
    public static class Utility
    {
        #region geometry stuff
        public static Box RotateAroundPoint(Box v, Box center, double angle)
        {
            var x = center.X + (v.X - center.X) * Math.Cos(angle) - (v.Y - center.Y) * Math.Sin(angle);
            var y = center.Y + (v.X - center.X) * Math.Sin(angle) + (v.Y - center.Y) * Math.Cos(angle);

            return new Box(x, y);
        }

        public static Vector RotateAroundPoint(Vector v, Vector center, double angle)
        {
            var x = center.X + (v.X - center.X) * Math.Cos(angle) - (v.Y - center.Y) * Math.Sin(angle);
            var y = center.Y + (v.X - center.X) * Math.Sin(angle) + (v.Y - center.Y) * Math.Cos(angle);

            return new Vector(x, y);
        }

        public static Vector PolarToRectangular(double r, double theta)
        {
            return new Vector(r * Math.Cos(theta), r * Math.Sin(theta));
        }

        public static bool HaveIntersection(Box start1, Box end1, Box start2, Box end2)
        {
            var denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));

            if (denom == 0)
                return false;

            var numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));
            var r = numer / denom;
            var numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));
            var s = numer2 / denom;

            if (r < 0 || r > 1 || s < 0 || s > 1)
                return false;

            return true;
        }
        #endregion

        //#region string stuff
        //public static string Compress(string s)
        //{
        //    var uncompressed = Encoding.Unicode.GetBytes(s);
        //    var compressed = QuickLZ.compress(uncompressed);
        //    return Convert.ToBase64String(compressed);
        //}

        //public static string Decompress(string s)
        //{
        //    var compressed = Convert.FromBase64String(s);
        //    var uncompressed = QuickLZ.decompress(compressed);
        //    return Encoding.Unicode.GetString(uncompressed, 0, uncompressed.Length);
        //}
        //#endregion
    }
}
