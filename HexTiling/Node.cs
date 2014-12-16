using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexTiling
{
    class Node
    {
        public string Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Text { get; set; }

        public Node()
        {
            Color = "red";
            Text = "";
        }

        public override string ToString()
        {
            return string.Format(@"\node[regular polygon,regular polygon sides=6, draw, thin, fill={0}, minimum size=2cm] at ({1},{2}) {{{3}}};", Color, X, Y, Text);
        }
    }
}
