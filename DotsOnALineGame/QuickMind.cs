using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsOnALineGame
{
    class QuickMind
    {
        public int NodesEvaluated { get; private set; }

        HashSet<Board> _redWonBoards = new HashSet<Board>();
        HashSet<Board> _greenWonBoards = new HashSet<Board>();

        public bool IsWin(Board board)
        {
            return IsRedWin(board);
        }

        bool IsRedWin(Board board)
        {
            NodesEvaluated++;
            if (_redWonBoards.Contains(board))
                return true;

            foreach (var move in board.GetAvailableMoves().EnumerateBits())
            {
                board.MakeMove(move, Color.Red);

                var redWon = board.IsRedWin;
                if (!redWon)
                    redWon = !IsGreenWin(board);

                board.UnMakeMove(move, Color.Red);

                if (redWon)
                {
                    _redWonBoards.Add(board.Clone());
                    return true;
                }
            }

            return false;
        }

        bool IsGreenWin(Board board)
        {
            NodesEvaluated++;
            if (_greenWonBoards.Contains(board))
                return true;

            var moved = false;
            foreach (var move in board.GetAvailableMoves().EnumerateBits())
            {
                moved = true;
                board.MakeMove(move, Color.Green);

                var greenWon = !IsRedWin(board);

                board.UnMakeMove(move, Color.Green);

                if (greenWon)
                {
                    _greenWonBoards.Add(board.Clone());
                    return true;
                }
            }

            if (!moved)
                _greenWonBoards.Add(board.Clone());

            return !moved;
        }
    }
}
