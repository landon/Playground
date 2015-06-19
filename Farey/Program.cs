using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farey
{
    class Program
    {
        const int Fairy = 1000;

        static void Main(string[] args)
        {
<<<<<<< HEAD
            var o = 3.5994694960212206;
            var result = "a:0.04509283819628633,b:0.04420866489832024,c:0.04509283819628667,d:0.04067197170645467,f:0.04067197170645467,g:0.04420866489832024,h:0.040671971706454674,j:0.018567639257294377,k:0.01945181255526091,l:0.02033598585322734,m:0.016799292661361626,s:0.006189213085764792";

=======
            var o = 3.591240875912409;
            var result = "a:0.06569343065693425,b:0.06569343065693427,c:0.058394160583941444,d:0.058394160583941444,f:0.05109489051094884,g:0.029197080291970764,h:0.02919708029197072,k:0.021897810218978107,s:0.007299270072992715";
            
>>>>>>> origin/master
            var r = Parse(result);
            var m = MaxBase(r);

            foreach (var x in r)
            {
                var t = Normalize(Farey(x.Value, Fairy), m);
                Console.WriteLine(x.Key + ": " + t.Item1);
            }

            var obj = Normalize(Reverse(Farey(1.0 / o, Fairy)), m);
            Console.WriteLine("obj: " + obj.Item1 + " / " + obj.Item2);

            Console.ReadKey();
        }

        static Tuple<int, int> Normalize(Tuple<int, int> x, int m)
        {
            return new Tuple<int, int>(m * x.Item1 / x.Item2, m);
        }

        static int MaxBase(Dictionary<string, double> r)
        {
            return r.Values.Max(x => Farey(x, Fairy).Item2);
        }

        static Tuple<int, int> Reverse(Tuple<int, int> x)
        {
            return new Tuple<int, int>(x.Item2, x.Item1);
        }


        static Dictionary<string, double> Parse(string s)
        {
            return s.Split(',').Select(x => x.Split(':')).ToDictionary(x => x[0], x => double.Parse(x[1]));
        }

        static Tuple<int, int> Farey(double x, int N)
        {
            var a = 0;
            var b = 1;
            var c = 1;
            var d = 1;

            while (b <= N && d <= N)
            {
                var m = (double)(a + c) / (b + d);
                if (x == m)
                {
                    if (b + d <= N)
                        return new Tuple<int, int>(a + c, b + d);
                    else if (d > b)
                        return new Tuple<int, int>(c, d);
                    else
                        return new Tuple<int, int>(a, b);
                }
                else if (x > m)
                {
                    a += c;
                    b += d;
                }
                else
                {
                    c += a;
                    d += b;
                }
            }

            if (b > N)
                return new Tuple<int, int>(c, d);
            else
                return new Tuple<int, int>(a, b);
        }
    }
}
