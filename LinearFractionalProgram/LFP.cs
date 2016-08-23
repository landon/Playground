using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearFractionalProgram
{
    class LFP
    {
        public string GLPK { get; private set; }
        public LFP(Matrix A, Matrix b, Matrix c, Matrix d, double alpha, double beta)
        {
            var objective = string.Join("+", Enumerable.Range(0, c.Columns).Where(i => c[0,i] != 0).Select(i => c[0, i] + "y" + i)) + "+" + alpha + "t";
            var constraints = "";

            for (int row = 0; row < A.Rows; row++)
                constraints += string.Join("+", Enumerable.Range(0, c.Columns).Where(i => A[row, i] != 0).Select(i => A[row, i] + "y" + i)) + "-" + b[row, 0] + "t" + "<= 0\n";

            constraints += string.Join("+", Enumerable.Range(0, c.Columns).Where(i => d[0, i] != 0).Select(i => d[0, i] + "y" + i)) + "+" + beta + "t = 1";

            var bounds = "t >= 0\ny0 >= 0\ny1>=0\ny2>=0\n-1000 <= y3 <= 1000";
            var generals = string.Join("\n", Enumerable.Range(0, c.Columns).Select(i => "y" + i)) + "\nt";

            var glpk = "maximize\n" + objective + "\nsubject to\n" + constraints + "\nbounds\n" + bounds + "\ngenerals\n" + generals + "\nend";
            GLPK = Normalize(glpk);
        }

        string Normalize(string s)
        {
            while (true)
            {
                var l = s.Length;
                s = s.Replace("++", "+");
                s = s.Replace("+-", "-");
                s = s.Replace("--", "+");
                s = s.Replace("-+", "-");
                if (l == s.Length)
                    break;
            }

            return s;
        }
    }
}
