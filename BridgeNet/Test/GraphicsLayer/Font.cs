using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public class Font
    {
        public string Name { get; set; }
        public int Size { get; set; }

        public Font(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}
