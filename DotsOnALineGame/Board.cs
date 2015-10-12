using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsOnALineGame
{
    enum Color
    {
        Red,
        Green
    }

    class Board
    {
        const byte WinPattern = 45;

        int N;
        Int64 Mask;
        Int64 Red;
        Int64 Green;

        public bool IsRedWin { get; private set; }

        public Board(int n)
        {
            N = n;
            Mask = (1L << n) - 1;
        }

        public Int64 GetAvailableMoves()
        {
            return ~Red & ~Green & Mask;
        }

        public void MakeMove(Int64 move, Color color)
        {
            if (color == Color.Red)
            {
                Red |= move;
                IsRedWin = CheckWin();
            }
            else
            {
                Green |= move;
            }
        }

        public void UnMakeMove(Int64 move, Color color)
        {
            if (color == Color.Red)
            {
                Red ^= move;
                IsRedWin = false;
            }
            else
            {
                Green ^= move;
            }
        }

        bool CheckWin()
        {
            var red = Red;
            while (red != 0)
            {
                var lsb = Int64Usage.LeastSignificantBit(red);
                red = red >> lsb;

                if ((red & WinPattern) == WinPattern)
                    return true;

                red = red >> 1;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (int)(Red | Green);
        }

        public override bool Equals(object obj)
        {
            return Equals((Board)obj);
        }

        public bool Equals(Board other)
        {
            return other.Red == Red && other.Green == Green;
        }
        
        public Board Clone()
        {
            return new Board(N) { Red = this.Red, Green = this.Green };
        }
    }
}
