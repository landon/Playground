using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class HashGraph
    {
        byte[] _values;
        List<int> _outMinusIn;
        int _hashCode;

        public HashGraph(Graph g, List<int> outMinusIn)
        {
            _values = new byte[g.N * (g.N - 1) / 2];
            _outMinusIn = outMinusIn;

            int k = 0;
            for (int i = 0; i < g.N; i++)
            {
                for (int j = i + 1; j < g.N; j++)
                {
                    var value = (byte)(g[i, j] ? 1 : 0);
                    _values[k] = value;

                    if (g[i, j])
                        _hashCode += 1 << (k % 32);

                    k++;
                }
            }

            foreach (var i in outMinusIn)
                _hashCode = _hashCode * 33 + i;
        }

        public override bool Equals(object obj)
        {
            var other = obj as HashGraph;
            if (other == null)
                return false;

            return _outMinusIn.SequenceEqual(other._outMinusIn) && _values.SequenceEqual(other._values);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
