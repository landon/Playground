using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearFractionalProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = MakeCodes(20, 2);
        }

        static string MakeCodes(int k, int qq)
        {
            var A = new Matrix(4, 0, -1, 0, 0, 0, 0, -1, 0, 0, qq, 0, 1, -(k - 1), -(k - 1), 0, -1, -2 * (k - 2), 0, -1, 0, -1, 1, 0, 0, -(k - 2), 1, 0, 0, -(k - 1), -(k - 3), -1, 0);
            var b = new Matrix(1, 0, -2, 0, -(k - 1), -6, k - 5, -2, -(k + 1));
            var c = new Matrix(4, -1, 0, -1.0 / (k - 1), 0);
            var d = new Matrix(4, -1, 5 - qq, -(k - 2) / (2.0 * (k - 1)), 0);

            var lfp = new LFP(A, b, c, d, 2, k + 1);
            return lfp.GLPK;
        }
    }
}
