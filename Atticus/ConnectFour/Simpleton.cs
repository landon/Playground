using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    class Simpleton : Player
    {
        protected Random RNG = new Random(DateTime.Now.Millisecond);
        public override int SelectMove(Board b, int lastColumn, int color)
        {
            var m = Win(b, color);
            if (m >= 0)
                return m;
            m = Win(b, -color);
            if (m >= 0)
                return m;
            return SelectNonWinningBlockingMove(b, lastColumn, color);
        }

        protected static int Win(Board b, int color)
        {
            foreach (var m in PossibleMoves(b).Where(column => b.IsWinningMove(new Move(column, color))))
                return m;
            return -1;
        }

        protected static List<int> PossibleMoves(Board b)
        {
            return Enumerable.Range(0, 7).Where(column => b.IsLegalMove(new Move(column, 1))).ToList();
        }

        protected virtual int SelectNonWinningBlockingMove(Board b, int lastColumn, int color)
        {
            var possibleMoves = PossibleMoves(b);
            return possibleMoves[RNG.Next(possibleMoves.Count)];
        }
    }
}
