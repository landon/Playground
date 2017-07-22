using System;
using System.Linq;

namespace ConnectFour
{
    class Program
    {
        static string Decimals = "0123456789";

        static void Main(string[] args)
        {
            var b = new Board();
            var player1 = new Human();
            var player2 = new Simpleton();

            PlayGame(player1, player2);
            Console.ReadKey();
        }

        static void PlayGame(Player p1, Player p2)
        {
            var b = new Board();
            b.Draw();
            var color = -1;
            var p = new[] { p1, p2 };

            while (true)
            {
                var column = p[(color + 1) / 2].Move(b, color);
                var m = new Move(column, color);
                var win = b.IsWinningMove(m);
                b.DoMove(m);
                b.Draw();

                if (win)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("good game");
                    break;
                }

                color = -color;
            }
        }
    }
}