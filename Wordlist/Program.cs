using System;
using System.Collections.Generic;
using System.IO;

namespace Wordlist
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> words;
            using (var sr = File.OpenText(@"c:\data\bip39wordlist.txt"))
                words = sr.ReadToEnd().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var vwp = new ValidWordPile(words);
            ///var sequence = "desk cinnamon collect cluster jazz float decade push boil food flag blame".Split(' ');

            foreach (var best in vwp.FindBestMatchingSequences(sequence, WordDistanceMetrics.LevenshteinDistance))
            {
                Console.WriteLine(string.Join(" ", best));
            }

            Console.ReadKey();
        }
    }
}