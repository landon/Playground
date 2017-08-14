using Algorithms.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms.WordGame.Optimized
{
    public class FastAccessibilityChecker
    {
        ulong[] _fixerResponses;
        int _fixerResponseCount;
        Dictionary<ulong, List<List<ulong>>> _breakerChoicesCache = new Dictionary<ulong, List<List<ulong>>>();

        public FastAccessibilityChecker(int n)
        {
            _fixerResponses = new ulong[1 << ((n + 1) >> 1)];
        }

        public bool IsAccessible(FastWord board, HashSet<FastWord> wonBoards)
        {
            for (int i = 0; i < board._length; i++)
            {
                for (int j = i + 1; j < board._length; j++)
                {
                    var x = board._trace[i];
                    var y = board._trace[j];
                    var swappable = x ^ y;
                    
                    var winningSwapAlwaysExists = true;
                    foreach (var breakerChoice in GetBreakerChoices(swappable))
                    {
                        var winningSwapExists = false;

                        GetFixerResponses(breakerChoice);
                        for (int k = 1; k < _fixerResponseCount; k++)
                        {
                            var childBoard = new FastWord(board._trace, i, j, _fixerResponses[k], board._stackCount);
                            if (wonBoards.Contains(childBoard))
                            {
                                winningSwapExists = true;
                                break;
                            }
                        }

                        if (!winningSwapExists)
                        {
                            winningSwapAlwaysExists = false;
                            break;
                        }
                    }

                    if (winningSwapAlwaysExists)
                        return true;
                }
            }

            return false;
        }

        void GetFixerResponses(List<ulong> possibleMoves)
        {
            _fixerResponseCount = 1 << possibleMoves.Count;

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
    }
}
