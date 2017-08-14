using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.DataStructures
{
    public class Bijection<D, C>
    {
        Dictionary<D, C> _f = new Dictionary<D, C>();
        Dictionary<C, D> _finverse = new Dictionary<C, D>();

        public C this[D d]
        {
            get { return _f[d]; }
            set 
            { 
                _f[d] = value;
                _finverse[value] = d;
            }
        }

        public D this[C c]
        {
            get { return _finverse[c]; }
            set 
            {
                _f[value] = c;
                _finverse[c] = value; 
            }
        }

        public IEnumerable<C> Apply(IEnumerable<D> e)
        {
            return e.Select(d => this[d]);
        }

        public IEnumerable<D> Apply(IEnumerable<C> e)
        {
            return e.Select(c => this[c]);
        }
    }

    public static class BijectionExtensions
    {
        public static Bijection<int, T> NumberObjects<T>(this IEnumerable<T> objects)
        {
            var numbering = new Bijection<int, T>();

            int i = 0;
            foreach (var o in objects)
            {
                numbering[i] = o;
                i++;
            }

            return numbering;
        }
    }
}
