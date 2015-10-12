using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotsOnALineGame
{
    class Program
    {
        static void Main(string[] args)
        {
            

            for (int i = 3; i < 100; i++)
            {
                var d = new DynamicProgramming(2 * i);
                var win = d.Solve();

                Console.WriteLine(2 * i + " : " + win);
            }

            Console.ReadKey();
        }
            
            //for (int i = 6; i < 100; i++)
            //{
            //    var board = new Board(i);
            //    var mind = new QuickMind();
            //    var win = mind.IsWin(board);

            //    Console.WriteLine(i + " : " + win + "  " + mind.NodesEvaluated);
            //}

            //Console.ReadKey();
        //}
    }
}
