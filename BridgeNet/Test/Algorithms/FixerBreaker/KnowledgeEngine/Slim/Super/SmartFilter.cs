using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SmartFilter : IWinFilter
    {
        public override IEnumerable<int> Filter(List<SuperSlimBoard> R, HashSet<SuperSlimBoard> W)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ColorPairOutcome> Analyze(SuperSlimBoard board)
        {
            var colorPairs = new List<Tuple<int, int>>();
            for (int i = 0; i < board._length; i++)
                for (int j = i + 1; j < board._length; j++)
                    colorPairs.Add(new Tuple<int, int>(i, j));

            return colorPairs.OrderBy(cp => (board._trace[cp.Item1] ^ board._trace[cp.Item2]).PopulationCount())
                             .Select(cp => new ColorPairOutcome() { Colors = cp, FixerOutcomes = AnalyzeColorPair(cp, board) });
        }

        IEnumerable<FixerOutcome> AnalyzeColorPair(Tuple<int, int> colors, SuperSlimBoard board)
        {
            var i = colors.Item1;
            var j = colors.Item2;

            var x = board._trace[i];
            var y = board._trace[j];
            var swappable = x ^ y;

            foreach (var breakerChoice in GetBreakerChoices(swappable))
            {
                GetFixerResponses(breakerChoice);
                var responses = Enumerable.Range(1, _fixerResponseCount - 1).Select(k => _fixerResponses[k]).Where(fr => fr.PopulationCount() <= 1);

                yield return new FixerOutcome() { BreakerChoice = breakerChoice, Exits = responses.Select(response => new SuperSlimBoard(board._trace, i, j, response, board._stackCount)).ToList() };
            }
        }

        #region guts
        ulong[] _fixerResponses = new ulong[8192];
        int _fixerResponseCount;
        Dictionary<ulong, List<List<ulong>>> _breakerChoicesCache = new Dictionary<ulong, List<List<ulong>>>();
        void GetFixerResponses(List<ulong> possibleMoves)
        {
            _fixerResponseCount = 1 << possibleMoves.Count;
            var tableLength = _fixerResponses.Length;
            while (tableLength <= _fixerResponseCount)
                tableLength *= 2;
            if (tableLength > _fixerResponses.Length)
                _fixerResponses = new ulong[tableLength];

            var subset = 1;
            while (subset < _fixerResponseCount)
            {
                var response = 0UL;
                var x = subset;

                while (x != 0)
                    response |= possibleMoves[Int32Usage.GetAndClearLeastSignificantBit(ref x)];

                _fixerResponses[subset] = response;
                subset++;
            }
        }

        List<List<ulong>> GetBreakerChoices(ulong swappable)
        {
            List<List<ulong>> choices;
            if (!_breakerChoicesCache.TryGetValue(swappable, out choices))
            {
                var bits = swappable.GetBits();
                var partitions = Algorithms.FixerBreaker.Chronicle.BranchGenerator.GetPartitions(bits.Count);
                choices = new List<List<ulong>>(partitions.Count);

                foreach (var partition in partitions)
                {
                    var choice = new List<ulong>(partition.Count);
                    choices.Add(choice);

                    foreach (var part in partition)
                    {
                        var x = 0UL;
                        foreach (var i in part)
                            x |= bits[i];

                        choice.Add(x);
                    }
                }

                _breakerChoicesCache[swappable] = choices;
            }

            return choices;
        } 
        #endregion
    }
}
