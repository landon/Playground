using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    class CopyCat : Simpleton
    {
        protected override int SelectNonWinningBlockingMove(Board b, int lastColumn, int color)
        {
            if (PossibleMoves(b).Contains(lastColumn))
                return lastColumn;
            return base.SelectNonWinningBlockingMove(b, lastColumn, color);
        }
    }
}
