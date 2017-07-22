using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    class Move
    {
        int _column;
        int _color;

        public Move(int column, int color)
        {
            _column = column;
            _color = color;
        }

        public int Column { get => _column; set => _column = value; }
        public int Color { get => _color; set => _color = value; }
    }
}
