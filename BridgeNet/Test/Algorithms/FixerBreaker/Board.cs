using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms.Utility;
using Algorithms.FixerBreaker.KnowledgeEngine;

namespace Algorithms.FixerBreaker
{
    public class Board : IEnumerable<long>
    {
        public static bool StorePermutationInfo;

#if DEBUG
        public List<string> StackStrings { get { return Stacks.Select(s => s.ToSetString()).ToList(); } }
        public string PotString { get { return Pot.ToSetString(); } }
#endif

        public List<long> Stacks;
        int _hashCode;
        List<int> PotSet;
        List<Int64> Traces;
        List<Tuple<int, Int64>> TracesForPermutationInfo;
        List<int> TraceChangeIndices;
        public Lazy<Template> Template;

        public long Pot;
        public int ColorCount;

        public Turn Turn;
        public int N;
        public Move LastMove;
        public long this[int x] { get { return Stacks[x]; } }

        public Board(List<long> stacks, long pot, bool computeHash = true)
        {
            Stacks = stacks;
            Pot = pot;
            Turn = Turn.Fixer;
            N = Stacks.Count;
            Template = new Lazy<Template>(() => new Template(Stacks.Select(s => s.PopulationCount()).ToList()));
            PotSet = Pot.ToSet();
            ColorCount = PotSet.Count;
            Traces = new List<Int64>(ColorCount);

            if (StorePermutationInfo)
            {
                TracesForPermutationInfo = new List<Tuple<int, Int64>>(ColorCount);
                TraceChangeIndices = new List<int>();
            }

            if (computeHash)
                ComputeHashCode();
        }

        void ComputeHashCode()
        {
            _hashCode = (int)(N);
            UpdateHash(ref _hashCode, (long)(Turn));

            Traces.Clear();
            foreach (var c in PotSet)
            {
                var t = GetTrace(c);
                if (t > 0)
                    Traces.Add(t);
            }

            Traces.Sort();

            foreach (var t in Traces)
                UpdateHash(ref _hashCode, t);

            if (StorePermutationInfo)
            {
                TracesForPermutationInfo.Clear();
                TraceChangeIndices.Clear();

                foreach (var c in PotSet)
                {
                    var t = GetTrace(c);
                    if (t > 0)
                        TracesForPermutationInfo.Add(new Tuple<int, Int64>(c, t));
                }

                TracesForPermutationInfo.Sort((x, y) => x.Item2.CompareTo(y.Item2));

                Int64 previousTrace = -1;
                for (int i = 0; i < TracesForPermutationInfo.Count; i++)
                {
                    var t = TracesForPermutationInfo[i];

                    if (t.Item2 != previousTrace)
                    {
                        TraceChangeIndices.Add(i);
                        previousTrace = t.Item2;
                    }
                }

                TraceChangeIndices.Add(TracesForPermutationInfo.Count);
            }
        }

        static void UpdateHash(ref int hash, long value)
        {
            hash = (int)(37 * hash + value);
        }

        Int64 GetTrace(int c)
        {
            var k = 1L << c;
            Int64 trace = 0;
            for (int i = 0; i < N; i++)
            {
                if ((Stacks[i] & k) != 0)
                    trace |= (1L << i);
            }

            return trace;
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

        public Board Clone()
        {
            var clone = new Board(Stacks.ToList(), Pot);
            clone.Turn = Turn;
            clone.LastMove = LastMove;
            clone._hashCode = _hashCode;

            clone.ComputeHashCode();

            return clone;
        }

        public Board DoMoveCombinationOnClone(IEnumerable<Move> swap)
        {
            var clone = new Board(Stacks.ToList(), Pot, false);
            clone.Turn = Turn;
            clone.LastMove = LastMove;

            clone.DoMoveCombination(swap);

            return clone;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Board);
        }

        public bool Equals(Board other)
        {
            if (other == null)
                return false;
            if (Turn != other.Turn)
                return false;
            if (N != other.N)
                return false;

            return Traces.SequenceEqual(other.Traces);
        }

        public Tuple<List<int>, List<int>> FindPermutation(Board other)
        {
            if (!StorePermutationInfo)
                throw new Exception("not storing permutation info");

            if (!TracesForPermutationInfo.Select(t => t.Item2).SequenceEqual(other.TracesForPermutationInfo.Select(t => t.Item2)))
                return null;

            var mappingPairs = new List<Tuple<List<int>, List<int>>>();
            for (int i = 0; i < TraceChangeIndices.Count - 1; i++)
            {
                var domain = new List<int>();
                var codomain = new List<int>();

                for (int j = TraceChangeIndices[i]; j < TraceChangeIndices[i + 1]; j++)
                {
                    domain.Add(TracesForPermutationInfo[j].Item1);
                    codomain.Add(other.TracesForPermutationInfo[j].Item1);
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

        bool SequenceEqualPermuted(Tuple<List<int>, List<int>> mapping, Board other)
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
