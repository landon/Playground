using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Algorithms.Utility
{
    public class Permutation
    {
        public Permutation(Factoradic f)
        {
            var sequence = new List<int>(f.Digits.Count);
            for (int i = 0; i < f.Digits.Count; i++)
                sequence.Add(i);

            for (int i = 0; i < f.Digits.Count; i++)
            {
                int j = f.Digits[f.Digits.Count - 1 - i] + i;

                sequence.Insert(i, sequence[j]);
                sequence.RemoveAt(j + 1);
            }

            Sequence = sequence;
            N = Sequence.Count;
        }

        public Permutation(List<int> sequence)
        {
            Sequence = new List<int>(sequence);
            N = Sequence.Count;

            var hash = new Dictionary<int, bool>();
            foreach (int i in sequence)
            {
                if (hash.ContainsKey(i))
                    throw new Exception("The supplied sequence has duplicates.");

                if (i < 0 || i > N - 1)
                    throw new Exception("The supplied sequence has elements that are out of range.");

                hash[i] = true;
            }
        }

        public List<T> Apply<T>(List<T> list)
        {
            if (list == null || list.Count != N)
                throw new Exception("The permutation cannot be applied to the given list.");

            var permutedList = new List<T>(N);

            for (int i = 0; i < N; i++)
                permutedList.Add(list[this[i]]);

            return permutedList;
        }

        public Permutation Inverse()
        {
            var sequence = new int[N];

            for (int i = 0; i < N; i++)
                sequence[this[i]] = i;

            return new Permutation(new List<int>(sequence));
        }

        public static Permutation operator *(Permutation p1, Permutation p2)
        {
            if (p1 == null || p2 == null)
                return null;

            if (p1.N != p2.N)
                throw new Exception("You tried to multiply permutations of different sets.");

            var sequence = new List<int>(p2.N);

            for (int i = 0; i < p2.N; i++)
                sequence.Add(p1[p2[i]]);

            return new Permutation(sequence);
        }

        public static bool operator ==(Permutation p1, Permutation p2)
        {
            if ((object)p1 == null && (object)p2 == null)
                return true;
            if ((object)p1 == null || (object)p2 == null)
                return false;

            return p1.Equals(p2);
        }

        public static bool operator !=(Permutation p1, Permutation p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return ToString() == obj.ToString();
        }

        public override string ToString()
        {
            return "(" + string.Join(" ", Sequence.Select(i => i + 1)) + ")";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static IEnumerable<Permutation> EnumerateAll(int n)
        {
            var count = Counting.Factorial(n);

            for (long i = 0; i < count; i++)
                yield return new Permutation(new Factoradic(i, n));
        }

        public int this[int i]
        {
            get
            {
                return Sequence[i];
            }
        }

        public List<int> Sequence
        {
            get;
            private set;
        }

        public int N
        {
            get;
            private set;
        }
    }
}