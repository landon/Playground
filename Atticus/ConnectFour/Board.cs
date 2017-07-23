using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    class Board
    {
        const int Rows = 6;
        const int Columns = 7;

        int[,] B = new int[Columns, Rows];

        public Board Copy()
        {
            var b = new Board();
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    b.B[column, row] = B[column, row];
                }
            }

            return b;
        }

        public void DoMove(Move m)
        {
            Move(B, m.Column, m.Color);
        }

        public bool IsWinningMove(Move m)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (B[m.Column, row] == 0)
                {
                    foreach(var triple in PossibleTriples(m.Column, row))
                    {
                        if (triple.All(p => B[p.Item1, p.Item2] == m.Color))
                            return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public bool IsLegalMove(Move m)
        {
            return IsLegalMove(B, m.Column, m.Color);
        }

        IEnumerable<List<Tuple<int, int>>> PossibleTriples(int column, int row)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                for (int dr = -1; dr <= 1; dr++)
                {
                    if (dc == 0 && dr == 0)
                        continue;

                    foreach (var triple in MakeTriple(column, row, dc, dr))
                    {
                        if (triple.All(IsValid))
                            yield return triple;
                    }

                }
            }
        }

        bool IsValid(Tuple<int, int> p)
        {
            return p.Item1 >= 0 && p.Item1 < Columns && p.Item2 >= 0 && p.Item2 < Rows;
        }

        IEnumerable<List<Tuple<int, int>>> MakeTriple(int column, int row, int dc, int dr)
        {
            yield return new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(column + dc, row + dr),
                new Tuple<int, int>(column + 2 * dc, row + 2 * dr),
                new Tuple<int, int>(column + 3 * dc, row + 3 * dr)
            };

            yield return new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(column - dc, row - dr),
                new Tuple<int, int>(column + dc, row + dr),
                new Tuple<int, int>(column + 2 * dc, row + 2 * dr)
            };
        }

        public void Draw()
        {
            DrawBoard(B);
            Console.WriteLine();
            Console.WriteLine();
        }

        void Move(int[,] b, int column, int color)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (b[column, row] == 0)
                {
                    b[column, row] = color;
                    break;
                }
            }
        }

        bool IsLegalMove(int[,] b, int column, int color)
        {
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (b[column, row] == 0)
                {
                    return true;
                }
            }

            return false;
        }

        static void DrawBoard(int[,] b)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {

                    Console.ForegroundColor = ConsoleColor.White;
                    if (b[column, row] > 0)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (b[column, row] < 0)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("O");
                }
                Console.WriteLine();
            }
        }
    }
}
