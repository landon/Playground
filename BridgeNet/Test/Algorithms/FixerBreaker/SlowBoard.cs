using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;
using Algorithms.FixerBreaker.KnowledgeEngine;

namespace Algorithms.FixerBreaker
{
    public class SlowBoard : IEnumerable<long>
    {
#if DEBUG
        public List<string> StackStrings { get { return Stacks.Select(s => s.ToSetString()).ToList(); } }
        public string PotString { get { return Pot.ToSetString(); } }
#endif

        public List<long> Stacks { get; private set; }
        int _hashCode;
        List<int> PotSet { get; set; }
        List<Tuple<int, int>> Degrees { get; set; }
        List<int> DegreeChangeIndices { get; set; }
        public Template Template { get; private set; }

        public long Pot { get; private set; }
        public int ColorCount { get; private set; }

        public Turn Turn { get; private set; }
        public int N { get; private set; }
        public Move LastMove { get; private set; }
        public long this[int x] { get { return Stacks[x]; } }

        public SlowBoard(List<long> stacks, long pot)
        {
            Stacks = stacks;
            Pot = pot;
            Turn = Turn.Fixer;
            N = Stacks.Count;
            Template = new Template(Stacks.Select(s => s.PopulationCount()).ToList());

            PotSet = Pot.ToSet();
            ColorCount = PotSet.Count();
            Degrees = new List<Tuple<int, int>>();
            DegreeChangeIndices = new List<int>();
            ComputeHashCode();
        }

        void ComputeHashCode()
        {
            _hashCode = (int)(N);
            UpdateHash(ref _hashCode, (long)(Turn));

            Degrees.Clear();
            DegreeChangeIndices.Clear();

            Degrees.AddRange(PotSet.Select(c => new Tuple<int, int>(c, Stacks.Count(s => s.IsBitSet(c)))).Where(x => x.Item2 > 0).OrderBy(x => x.Item2));

            var previousDegree = -1;
            for (int i = 0; i < Degrees.Count; i++)
            {
                var degree = Degrees[i];

                UpdateHash(ref _hashCode, degree.Item2);

                if (degree.Item2 != previousDegree)
                {
                    DegreeChangeIndices.Add(i);
                    previousDegree = degree.Item2;
                }
            }

            DegreeChangeIndices.Add(Degrees.Count);
        }

        static void UpdateHash(ref int hash, long value)
        {
            hash = (int)(100 * hash + value);
        }

        public void MakeMove(Move move)
        {
            LastMove = move;

            if (move.Stack >= 0)
                Stacks[move.Stack] = Stacks[move.Stack].FlipBits(move.Added, move.Removed);

            Turn = OtherTurn(Turn);

            ComputeHashCode();
        }

        public void DoMoveCombination(IEnumerable<Move> moves)
        {
            foreach (var move in moves)
                Stacks[move.Stack] = Stacks[move.Stack].FlipBits(move.Added, move.Removed);

            ComputeHashCode();
        }

        public SlowBoard Clone()
        {
            var clone = new SlowBoard(Stacks.ToList(), Pot);
            clone.Turn = Turn;
            clone.LastMove = LastMove;
            clone._hashCode = _hashCode;

            clone.ComputeHashCode();

            return clone;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SlowBoard);
        }

        public bool Equals(SlowBoard other)
        {
            if (other == null)
                return false;
            if (Turn != other.Turn)
                return false;
            if (N != other.N)
                return false;

            if (Stacks.SequenceEqual(other.Stacks))
            {
                if (Turn == FixerBreaker.Turn.Fixer)
                    return true;

                if (LastMove.Stack == other.LastMove.Stack &&
                   (LastMove.Added == other.LastMove.Added && LastMove.Removed == other.LastMove.Removed ||
                    LastMove.Added == other.LastMove.Removed && LastMove.Removed == other.LastMove.Added))
                    return true;

                return false;
            }

            return FindPermutation(other) != null;
        }

        public Tuple<List<int>, List<int>> FindPermutation(SlowBoard other)
        {
            if (!Degrees.Select(x => x.Item2).SequenceEqual(other.Degrees.Select(x => x.Item2)))
                return null;

            var mappingPairs = new List<Tuple<List<int>, List<int>>>();
            for (int i = 0; i < DegreeChangeIndices.Count - 1; i++)
            {
                var domain = new List<int>();
                var codomain = new List<int>();

                for (int j = DegreeChangeIndices[i]; j < DegreeChangeIndices[i + 1]; j++)
                {
                    domain.Add(Degrees[j].Item1);
                    codomain.Add(other.Degrees[j].Item1);
                }

                mappingPairs.Add(new Tuple<List<int>, List<int>>(domain, codomain));
            }

            return mappingPairs.Select(mp => Permutation.EnumerateAll(mp.Item2.Count).Select(p => new Tuple<List<int>, List<int>>(mp.Item1, p.Apply(mp.Item2))))
                               .CartesianProduct()
                               .Select(mapping => FlattenMapping(mapping))
                               .FirstOrDefault(mapping => SequenceEqualPermuted(mapping, other));
        }

        Tuple<List<int>, List<int>> FlattenMapping(IEnumerable<Tuple<List<int>, List<int>>> mapping)
        {
            return new Tuple<List<int>, List<int>>(mapping.SelectMany(x => x.Item1).ToList(), mapping.SelectMany(x => x.Item2).ToList());
        }

        bool SequenceEqualPermuted(Tuple<List<int>, List<int>> mapping, SlowBoard other)
        {
            for (int i = 0; i < Stacks.Count; i++)
            {
                var permutedStack = ApplyMapping(mapping, Stacks[i]);
                if (permutedStack != other.Stacks[i])
                    return false;
            }

            if (Turn == FixerBreaker.Turn.Fixer)
                return true;

            if (LastMove.Stack != other.LastMove.Stack)
                return false;

            var ours = new[] { LastMove.Added, LastMove.Removed };
            var theirs = new List<int>() { other.LastMove.Added, other.LastMove.Removed };

            return ApplyMapping(mapping, ours).ToList().Equal(theirs);
        }

        static long ApplyMapping(Tuple<List<int>, List<int>> mapping, long stack)
        {
            return ApplyMapping(mapping, stack.ToSet()).ToInt64();
        }

        public static IEnumerable<int> ApplyMapping(Tuple<List<int>, List<int>> mapping, IEnumerable<int> set)
        {
            return set.Select(c => mapping.Item2[mapping.Item1.IndexOf(c)]);
        }
        public static IEnumerable<int> ApplyInverseMapping(Tuple<List<int>, List<int>> mapping, IEnumerable<int> set)
        {
            return set.Select(c => mapping.Item1[mapping.Item2.IndexOf(c)]);
        }

        public bool ExactlyEqual(SlowBoard other)
        {
            if (other == null)
                return false;
            if (Turn != other.Turn)
                return false;
            if (N != other.N)
                return false;
            if (Pot != other.Pot)
                return false;

            if (Stacks.SequenceEqual(other.Stacks))
            {
                if (Turn == FixerBreaker.Turn.Fixer)
                    return true;

                if (LastMove.Stack == other.LastMove.Stack &&
                   (LastMove.Added == other.LastMove.Added && LastMove.Removed == other.LastMove.Removed ||
                    LastMove.Added == other.LastMove.Removed && LastMove.Removed == other.LastMove.Added))
                    return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static Turn OtherTurn(Turn turn)
        {
            return 1 - turn;
        }

        public IEnumerator<long> GetEnumerator()
        {
            return Stacks.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("|", Stacks.Select(x => x.ToSetStringSmall()));
        }

        public string ToTex()
        {
            return Stacks.Select((stack, i) => "$L(v_" + (i + 1) + ") = " + stack.ToSetString().Replace("{", "\\{").Replace("}", "\\}") + "$").JoinPretty(", ");
        }
    }
}
