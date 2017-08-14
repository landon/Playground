using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;

namespace Algorithms.WordGame
{
    public class AccessibilityChecker
    {
        List<char> _alphabet;
        public AccessibilityChecker(IEnumerable<char> alphabet)
        {
            _alphabet = alphabet.ToList();
        }

        public bool IsAccessible(string w, List<string> S)
        {
            foreach(var c in _alphabet)
            {
                var goodChoiceAlwaysExists = true;

                var swappers = _alphabet.Except(new[] { c }).ToList();
                foreach(var partition in EnumeratePartitions(w, swappers))
                {
                    var goodChoiceExists = false;
                    foreach (var choice in Enumerable.Range(0, partition.Count).ToList().GenerateSublists())
                    {
                        if (choice.Count <= 0)
                            continue;

                        var ww = MakeMove(w, partition, choice, swappers);
                        if (S.Contains(ww))
                        {
                            goodChoiceExists = true;
                            break;
                        }
                    }

                    if (!goodChoiceExists)
                    {
                        goodChoiceAlwaysExists = false;
                        break;
                    }
                }

                if (goodChoiceAlwaysExists)
                    return true;
            }

            return false;
        }

        IEnumerable<List<List<int>>> EnumeratePartitions(string w, List<char> swappers)
        {
            var chars = w.ToCharArray();
            var indices = chars.IndicesWhere(c => swappers.Contains(c)).ToList();

            var indexPartitions = Algorithms.FixerBreaker.Chronicle.BranchGenerator.GetPartitions(indices.Count);
            foreach (var indexPartition in indexPartitions)
            {
                var partition = new List<List<int>>(indexPartition.Count);
                foreach (var indexPart in indexPartition)
                {
                    var part = indexPart.Select(i => indices[i]).ToList();
                    partition.Add(part);
                }

                yield return partition;
            }
        }

        string MakeMove(string w, List<List<int>> partition, List<int> choice, List<char> swappers)
        {
            var chars = w.ToCharArray();
            foreach (var j in choice.SelectMany(i => partition[i]))
                chars[j] = swappers[1 - swappers.IndexOf(chars[j])];

            return new string(chars);
        }
    }
}
