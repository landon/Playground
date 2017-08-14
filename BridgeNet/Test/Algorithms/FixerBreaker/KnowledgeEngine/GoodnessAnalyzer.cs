using Algorithms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.FixerBreaker.KnowledgeEngine
{
    public class GoodnessAnalyzer : SwapAnalyzer
    {
        protected override string ChildReason { get { return "goodness increased"; } }

        public GoodnessAnalyzer(bool findCleanestWin = true)
            : base(findCleanestWin)
        {
        }

        protected override bool IsChildBoardBetter(Knowledge knowledge, Board board, Board childBoard)
        {
            var x = MetaKnowledge.GetColorGraphs(childBoard.Stacks, childBoard.Pot).Select(c => c.Count).ToList();
            var y = MetaKnowledge.GetColorGraphs(board.Stacks, board.Pot).Select(c => c.Count).ToList();

            return StandardDeviation(x) < StandardDeviation(y);
        }

        double Mean(List<int> x)
        {
            return x.Sum() / x.Count;
        }

        double Median(List<int> x)
        {
            return x.OrderBy(a => a).ElementAt(x.Count / 2);
        }

        double StandardDeviation(List<int> x)
        {
            var mu = Mean(x);

            return Math.Sqrt(x.Sum(a => (a - mu) * (a - mu)) / x.Count);
        }

        double AbsoluteDeviation(List<int> x)
        {
            var mu = Median(x);

            return x.Sum(a => Math.Abs(mu - a));
        }

        bool PushLeftOrdering(Knowledge knowledge, Board board, Board childBoard)
        {
            var x = GetVectors(childBoard);
            var y = GetVectors(board);

            for (int i = 0; i < Math.Min(x.Count, y.Count); i++)
            {
                var c = CompareVectors(x[i], y[i]);

                if (c > 0)
                    return true;
                if (c < 0)
                    return false;
            }

            return x.Count < y.Count;
        }

        List<List<int>> GetVectors(Board b)
        {
            return MetaKnowledge.GetColorGraphs(b.Stacks, b.Pot).OrderByDescending(c => c.Count).ToList();
        }

        int CompareVectors(List<int> x, List<int> y)
        {
            if (x.Count > y.Count)
                return -1;
            if (x.Count < y.Count)
                return 1;

            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] < y[i])
                    return 1;
                if (x[i] > y[i])
                    return -1;
            }

            return 0;
        }
    }
}
