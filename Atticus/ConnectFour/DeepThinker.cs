using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectFour
{
    class DeepThinker : Simpleton
    {
        protected override int SelectNonWinningBlockingMove(Board b, int lastColumn, int color)
        {
            var m = WinInXMoves(b, color, 4);
            if (m >= 0)
                return m;

            var nonLosers = new List<int>();
            foreach (var column in PossibleMoves(b))
            {
                var copy = b.Copy();
                copy.DoMove(new Move(column, color));

                var t = WinInXMoves(copy, -color, 3);
                if (t < 0)
                    nonLosers.Add(column);
            }

            if (nonLosers.Count > 0)
            {
                return nonLosers[RNG.Next(nonLosers.Count)];
            }

            return base.SelectNonWinningBlockingMove(b, lastColumn, color);
        }

        int WinInXMoves(Board b, int color, int X)
        {
            var pm = PossibleMoves(b);
            foreach (var column in pm)
            {
                if (b.IsWinningMove(new Move(column, color)))
                    return column;
            }

            var oppWins = 0;
            foreach (var column in PossibleMoves(b))
            {
                if (b.IsWinningMove(new Move(column, -color)))
                {
                    pm = new List<int>() { column };
                    oppWins++;
                }
            }

            if (oppWins > 1)
                return -1;

            foreach (var column in pm)
            {
                var copy = b.Copy();
                copy.DoMove(new Move(column, color));

                var winner = true;
                foreach (var column2 in PossibleMoves(copy))
                {
                    var copy2 = copy.Copy();
                    copy2.DoMove(new Move(column2, -color));

                    int m;

                    if (X <= 1)
                        m = Win(copy2, color);
                    else
                    {
                        m = WinInXMoves(copy2, color, X - 1);
                    }
                    if (m < 0)
                    {
                        winner = false;
                        break;
                    }
                }

                if (winner)
                    return column;
            }

            return -1;
        }
    }
}
