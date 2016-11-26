using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int n = 1; n < 7; n++)
            {
                Console.WriteLine(string.Join(", ", new FareySequence(n)));
            }

            Console.ReadKey();
        }
    }
}
