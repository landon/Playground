using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    class OneMoveLookAhead : CopyCat
    {
        protected override int SelectNonWinningBlockingMove(Board b, int lastColumn, int color)
        {
            var nonLosers = new List<int>();
            foreach (var column in PossibleMoves(b))
            {
                var copy = b.Copy();
                copy.DoMove(new Move(column, color));
                var m = Win(copy, -color);
                if (m >= 0)
                    continue;
                nonLosers.Add(column);

                var winner = true;
                foreach (var column2 in PossibleMoves(copy))
                {
                    var copy2 = copy.Copy();
                    m = Win(copy2, color);
                    if (m < 0)
                    {
                        winner = false;
                        break;
                    }
                }

                if (winner)
                    return column;
            }

            if (nonLosers.Count > 0)
            {
                return nonLosers[RNG.Next(nonLosers.Count)];
            }

            return base.SelectNonWinningBlockingMove(b, lastColumn, color);
        }
    }
}
