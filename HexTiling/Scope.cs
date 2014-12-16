using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexTiling
{
    class Scope
    {
        double _x;
        double _y;
        List<Node> _nodes = new List<Node>();

        public Scope(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public void AddNode(string color, double x, double y, string text)
        {
            _nodes.Add(new Node() { Color = color, X = x, Y = y, Text = text });
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format(@"\begin{{scope}}[shift={{({0},{1})}}]", _x, _y));

            foreach (var node in _nodes)
                sb.AppendLine(node.ToString());

            sb.AppendLine(@"\end{scope}");
            return sb.ToString();
        }
    }
}
