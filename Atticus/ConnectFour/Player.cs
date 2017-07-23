using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    abstract class Player
    {
        public abstract int SelectMove(Board b, int lastColumn, int color);
    }
}
