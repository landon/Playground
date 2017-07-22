using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    class Simpleton : Player
    {
        Random RNG = new Random(DateTime.Now.Millisecond);
        public override int Move(Board b, int color)
        {
            var possibleMoves = Enumerable.Range(0, 7).Where(column => b.IsLegalMove(new Move(column, color))).ToList();

            if (possibleMoves.Any(column => b.IsWinningMove(new Move(column, color))))
            {
                return possibleMoves.First(column => b.IsWinningMove(new Move(column, color)));
            }
            if (possibleMoves.Any(column => b.IsWinningMove(new Move(column, -color))))
            {
                return possibleMoves.First(column => b.IsWinningMove(new Move(column, -color)));
            }

            return possibleMoves[RNG.Next(possibleMoves.Count)];
        }
    }
}
