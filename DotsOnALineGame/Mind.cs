using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotsOnALineGame
{
    class Mind
    {
        const int MaxScore = 10000;
        const int MaxPly = 30;

        List<Int64[]> PrincipalVariation = new List<Int64[]>();
        int[] PrincipalVariationLength = new int[MaxPly];

        public int WinDepth(Board board)
        {
            return MinimalRedWinDepth(board, 0, MaxScore, 0);
        }

        int MinimalRedWinDepth(Board board, int currentPly, int alpha, int beta)
        {
            SetupPrincipleVariation(currentPly);

            Int64 lastMove = -1;
            foreach (var move in board.GetAvailableMoves().EnumerateBits())
            {
                board.MakeMove(move, Color.Red);

                int score;
                var redWon = board.IsRedWin;
                if (board.IsRedWin)
                {
                    score = currentPly;
                }
                else
                {
                    score = MaximalRedWinDepth(board, currentPly + 1, beta, alpha);
                }

                board.UnMakeMove(move, Color.Red);

                if (score < alpha)
                {
                    alpha = score;
                    UpdatePrincipalVariation(move, currentPly);
                }

                if (score <= beta || score <= currentPly)
                {
                    return score;
                }

                lastMove = move;
            }

            if (alpha >= MaxScore)
                UpdatePrincipalVariation(lastMove, currentPly);


            return alpha;
        }

        int MaximalRedWinDepth(Board board, int currentPly, int alpha, int beta)
        {
            SetupPrincipleVariation(currentPly);

            Int64 lastMove = -1;
            foreach (var move in board.GetAvailableMoves().EnumerateBits())
            {
                board.MakeMove(move, Color.Green);

                var score = MinimalRedWinDepth(board, currentPly + 1, beta, alpha);

                board.UnMakeMove(move, Color.Green);

                if (score > alpha)
                {
                    alpha = score;
                    UpdatePrincipalVariation(move, currentPly);
                }

                if (score >= beta)
                {
                    return score;
                }

                lastMove = move;
            }

            if (lastMove == -1)
            {
                UpdatePrincipalVariation(lastMove, currentPly);

                return MaxScore;
            }

            return alpha;
        }

        void SetupPrincipleVariation(int ply)
        {
            if (PrincipalVariation.Count <= ply)
                PrincipalVariation.Add(new Int64[MaxPly]);

            PrincipalVariationLength[ply] = ply;
            PrincipalVariationLength[ply + 1] = ply + 1;
        }

        void UpdatePrincipalVariation(Int64 move, int currentPly)
        {
            PrincipalVariation[currentPly][currentPly] = move;

            for (int i = currentPly + 1; i < PrincipalVariationLength[currentPly + 1]; i++)
                PrincipalVariation[currentPly][i] = PrincipalVariation[currentPly + 1][i];

            PrincipalVariationLength[currentPly] = PrincipalVariationLength[currentPly + 1];
        }
    }
}
