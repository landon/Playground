using System;
using System.Collections.Generic;
using System.Linq;

namespace Wordlist
{
    public class ValidWordPile
    {
        readonly IEnumerable<string> _words;

        public ValidWordPile(IEnumerable<string> words)
        {
            _words = words;
        }

        public IEnumerable<IEnumerable<string>> FindBestMatchingSequences(IEnumerable<string> sequence, Func<string, string, int> distance)
        {
            return sequence.Select(word => FindBestMatches(word, distance)).CartesianProduct();
        }

        public List<string> FindBestMatches(string word, Func<string, string, int> distance)
        {
            var closest = new List<string>();
            var minDistance = int.MaxValue;

            foreach (var w in _words)
            {
                var d = distance(w, word);
                if (d < minDistance)
                {
                    minDistance = d;
                    closest.Clear();
                }

                if (d <= minDistance)
                    closest.Add(w);
            }

            return closest;
        }
    }
}
