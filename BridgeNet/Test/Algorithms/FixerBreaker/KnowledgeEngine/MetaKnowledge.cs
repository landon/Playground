using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public static class MetaKnowledge
    {
        public static readonly int Infinity = int.MaxValue;

        public static bool Exists(this object o)
        {
            return o != null;
        }

        public static IEnumerable<int> Naturals()
        {
            return Enumerable.Range(0, Infinity);
        }

        public static IEnumerable<int> Interval(int first, int last)
        {
            return Enumerable.Range(first, last - first + 1);
        }

        public static int? TryParseInt(this string s)
        {
            int x;
            if (int.TryParse(s, out x))
                return x;

            return null;
        }
        public static double? TryParseDouble(this string s)
        {
            double x;
            if (double.TryParse(s, out x))
                return x;

            return null;
        }


        public static bool DegreeCondition(this Graph g, Board b, Func<List<int>, int> missingEdges = null)
        {
            return DegreeCondition(g, b.Stacks, b.Pot);
        }

        public static bool DegreeCondition(this Graph g, List<long> stacks, long pot, Func<List<int>, int> missingEdges = null)
        {
            if (missingEdges == null)
                missingEdges = Y => 0;

            var colorGraphs = GetColorGraphs(stacks, pot);

            foreach (var X in ListUtility.EnumerateSublists(g.Vertices))
            {
                var e = g.EdgesOn(X) - missingEdges(X);

                if (e <= 0)
                    continue;

                var value = colorGraphs.Sum(cg => cg.IntersectionCount(X) / 2);

                if (value < e)
                    return false;
            }

            return true;
        }

        public static List<List<int>> GetColorGraphs(List<long> stacks, long pot)
        {
            return pot.EnumerateBits().Select(alpha => G_alpha(alpha, stacks).ToList()).ToList();
        }

        public static IEnumerable<int> G_alpha(int alpha, List<long> stacks)
        {
            return stacks.IndicesWhere(stack => stack.IsBitSet(alpha));
        }

        public static int d_H(int alpha, IEnumerable<long> stacksInH)
        {
            return stacksInH.Count(stack => stack.IsBitSet(alpha));
        }

        public static int Surplus(this Board board, Graph g)
        {
            return GetColorGraphs(board.Stacks, board.Pot).Sum(cg => cg.Count / 2) - g.E;
        }

        public static int Surplus(this Board board, Graph g, List<int> vertices)
        {
            return GetColorGraphs(board.Stacks, board.Pot).Sum(cg => cg.IntersectionCount(vertices) / 2) - g.EdgesOn(vertices);
        }

        public static void ToConsole(this string s)
        {
        }

        public static Move MapMove(Tuple<List<int>, List<int>> f, Move move)
        {
            var mappedMove = new Move() { Stack = move.Stack };
            var m = Board.ApplyMapping(f, new[] { move.Added, move.Removed }).ToList();

            mappedMove.Added = m[0];
            mappedMove.Removed = m[1];

            return mappedMove;
        }
    }
}
