using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class TransitivePartition
    {
        public List<Part> Parts { get; private set; }
        public TransitivePartition(List<int> set)
        {
            Parts = new List<Part>() { new Part() { Set = set, Scores = new List<int>() } };
        }

        public void Refine(int[] score, int scoreNumber)
        {
            if (Parts[0].Scores.Count >= scoreNumber)
                return;

            var refined = new List<Part>(Parts.Count);
            foreach (var part in Parts)
            {
                var map = RefinePart(part, score);
                foreach (var kvp in map)
                {
                    var p = new Part();
                    p.Set = kvp.Value;
                    p.Scores = new List<int>(part.Scores);
                    p.Scores.Add(kvp.Key);

                    refined.Add(p);
                }
            }

            Parts = refined;
        }

        Dictionary<int, List<int>> RefinePart(Part part, int[] score)
        {
            var map = new Dictionary<int, List<int>>();
            foreach (var v in part.Set)
            {
                var x = score[v];
                List<int> l;
                if (!map.TryGetValue(x, out l))
                {
                    l = new List<int>();
                    map[x] = l;
                }

                l.Add(v);
            }

            return map;
        }

        public override bool Equals(object obj)
        {
            var tp = obj as TransitivePartition;
            if (tp == null)
                return false;

            foreach (var part in Parts)
            {
                var p = tp.FindPartWithScores(part.Scores);
                if (p == null)
                    return false;

                if (p.Set.Count != part.Set.Count)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        Part FindPartWithScores(List<int> scores)
        {
            foreach (var part in Parts)
            {
                if (Enumerable.SequenceEqual(part.Scores, scores))
                    return part;
            }

            return null;
        }

        public class Part
        {
            public List<int> Set { get; set; }
            public List<int> Scores { get; set; }
        }
    }
}
