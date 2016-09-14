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
            var list = new List<int>() { 6, 7, 8, 9, 10, 15, 20 };

            foreach (var k in list)
            {
                if (k <= 6)
                    Console.WriteLine(k + " : " + NewerBoundSmallK(k));
                else
                    Console.WriteLine(k + " : " + NewerBoundLargeK(k));
            }


            Console.ReadKey();
        }

        static double NewerBoundSmallK(int k)
        {
            return 5 + 93.0 / 766.0;
        }

        static double NewerBoundLargeK(int k)
        {
            return k - 1.0 + Math.Pow((k - 3.0), 2) * (2.0*k-3.0) / (Math.Pow(k, 4) - 2.0 * Math.Pow(k, 3) - 11.0 * Math.Pow(k, 2) + 28.0 * k - 14.0);
        }

        static double NewBoundSmallK(int k)
        {
            return k - 1.0 + (k - 3.0) / (k * k - 2 * k + 2.0);
        }

        static double DischargingBound56(int k)
        {
            return k - 1.0 + (k - 3.0) * (2.0 * k - 5.0) / (Math.Pow(k, 3.0) + 2*Math.Pow(k, 2.0) - 18.0 * k + 15.0);
        }

        static double DischargingBound(int k)
        {
            return k - 1.0 + (k - 3.0) * (2.0 * k - 5.0) / (Math.Pow(k, 3.0) + Math.Pow(k, 2.0) - 15.0 * k + 15.0);
        }

        static double SuperBestPossible(int k)
        {
            return g_avg(k, SuperBestPossibleC(k));
        }

        static double SuperBestPossibleC(int k)
        {
            return k / 2.0 + 1.0 / (k - 1);
        }

        static double BestPossible(int k)
        {
            return g_avg(k, BestPossibleC(k));
        }

        static double BestPossibleC(int k)
        {
            if (k <= 6)
                return (k - 4) * (1.0 / 2.0 + 1.0 / (k - 1));

            return (k - 3) * (1.0 / 2.0 + 1.0 / (k - 1));
        }

        static double LatestBound(int k)
        {
            return g_avg(k, LatestBoundC(k));
        }

        static double LatestBoundC(int k)
        {
            if (k <= 6)
                return (k - 4) * (1.0 / 2.0 + (k - 5.0) / (3.0 * (k - 2) * (k - 3)));

            return (k - 3) * (1.0 / 2.0 + (k - 5.0) / (3.0 * (k - 2) * (k - 3)));
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
