using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InequalityExplorer
{
    class Atom : Expression
    {
        public string Name { get; }

        public Atom(string name)
        {
            Name = name;
        }

        public override void Simplify()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
