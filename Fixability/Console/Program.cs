using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fixability;

namespace Console
{
    public class Program
    {
        public static void Main()
        {
            var g = new Fixability.Basic.Graph();
            var mind = new Mind<List<int>, List<int>, Fixability.Basic.Graph>(g, new DynamicAnalyzer<List<int>,List<int>>(), new StaticAnalyzer<List<int>,List<int>>(), null);
        }
    }
}
