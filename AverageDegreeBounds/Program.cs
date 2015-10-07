using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AverageDegreeBounds
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ComputeBetterOreBound(8));
            Console.ReadKey();
        }

        static double ComputeBetterOreBound(int delta)
        {
            return alpha(delta + 1) * (delta - 2) + (2 - alpha(delta + 1)) * (delta - 1) * (delta - 5) / (delta) / (delta - 3);
        }

        static double ComputeOreBound(int delta)
        {
            return alpha(delta + 1) * (delta - 2) + 2 * (1 - alpha(delta + 1)) * (delta - 1) * (delta - 5) / (delta) / (delta - 3);
        }

        static void PrintBounds()
        {
            for (int k = 4; k <= 20; k++)
            {
                Console.WriteLine(string.Format("{3} & {0:0.0000}\t\t{1:0.0000}\t\t{2:0.0000}\t\t{4:0.0000}", gallai(k), ks(k), ours(k), k, best(k)));
            }
        }

        static double best(int k)
        {
            return g_avg(k, k / 2.0 + 1.0 / (k - 1));
        }

        static double ks(int k)
        {
            if (k < 9)
                return 0;

            return g_avg(k, (k - 4.0) * alpha(k) / 3.0);
        }

        static double gallai(int k)
        {
            return g_avg(k, 0);
        }

        static double ours(int k)
        {
            if (k >= 7)
                return g_avg(k, (k - 3.0) * alpha(k));
            return g_avg(k, (k - 4.0) * alpha(k));
        }

        static double g_avg(int k, double c)
        {
            return k - 1.0 + (k - 3.0) / ((k - c) * (k - 1.0) + k - 3.0);
        }

        static double alpha(int k)
        {
            return 0.5 - 1.0 / (k - 1) / (k - 2);
        }
    }
}
