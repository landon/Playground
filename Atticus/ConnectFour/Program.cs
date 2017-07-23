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
            var player1 = new DeepThinker();
            var player2 = new Human();

            PlayGame(player1, player2);
            Console.ReadKey();
        }

        static void PlayGame(Player p1, Player p2)
        {
            var b = new Board();
            b.Draw();
            var color = -1;
            var p = new[] { p1, p2 };
            var column = -1;

            while (true)
            {
                column = p[(color + 1) / 2].SelectMove(b, column, color);
                var m = new Move(column, color);
                var win = b.IsWinningMove(m);
                Console.WriteLine("playing " + m.Column);
                Console.WriteLine();
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