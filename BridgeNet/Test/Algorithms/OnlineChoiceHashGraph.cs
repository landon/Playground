using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    class OnlineChoiceHashGraph
    {
        int[] _f;
        int[] _g;
        int _hashCode;

        public OnlineChoiceHashGraph(int[] f, int[] g)
        {
            _f = (int[])f.Clone();
            _g = (int[])g.Clone();

            for (int i = 0; i < _g.Length; i++)
            {
                if (_g[i] == 0)
                    _f[i] = 0;
            }

            foreach (var i in _f)
                _hashCode = _hashCode * 33 + i;

            foreach(var i in _g)
                _hashCode = _hashCode * 33 + i;
        }

        public override bool Equals(object obj)
        {
            var other = obj as OnlineChoiceHashGraph;
            if (other == null)
                return false;

            return _g.SequenceEqual(other._g) && _f.SequenceEqual(other._f);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
