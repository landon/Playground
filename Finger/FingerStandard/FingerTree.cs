using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public abstract class FingerTree<T, M> : ISplittable<T, M> where T : IMeasured<M>
    {
        public abstract Monoid<M> ConcreteMonoid { get; }
        public abstract M Measure();

        public abstract FingerTree<T, M> PushLeft(T t);
        public abstract FingerTree<T, M> PushRight(T t);

        public abstract IEnumerable<T> ToSequenceLeft();
        public abstract IEnumerable<T> ToSequenceRight();

        public abstract View<T, M> LeftView();
        public abstract View<T, M> RightView();

        public abstract FingerTree<T, M> Merge(FingerTree<T, M> f);
        public abstract FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> f);

        public abstract Split<FingerTree<T, M>, T, M> Split(Func<M,bool> p, M a);
        public abstract Pair<FingerTree<T, M>, FingerTree<T, M>> Split(Func<M, bool> p);

        public FingerTree<T, M> TakeUntil(Func<M, bool> p) => Split(p).Left;
        public FingerTree<T, M> DropUntil(Func<M, bool> p) => Split(p).Right;
        public T Lookup(Func<M, bool> p, M a) => DropUntil(p).LeftView().End;

        public static FingerTree<T, M> FromSequence(IEnumerable<T> sequence, Monoid<M> m)
        {
            var sequenceEnum = sequence.GetEnumerator();
            FingerTree<T, M> r = new EmptyFingerTree<T, M>(m);

            while (sequenceEnum.MoveNext())
                r = r.PushRight(sequenceEnum.Current);

            return r;
        }

        public static FingerTree<T, M> Create(List<T> left, FingerTree<Node<T, M>, M> f, Digit<T, M> right)
        {
            var m = right.M;

            if (left.Count > 0)
                return new DeepFingerTree<T, M>(m, new Digit<T, M>(m, left), f, right);

            if (f is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(right.Digits, m);

            var il = f.LeftView();
            var n = il.End.Nodes;

            return new DeepFingerTree<T, M>(m, new Digit<T, M>(m, n), il.Rest, right);
        }

        public virtual FingerTree<T, M> Reverse()
        {
            return this;
        }

        public static Digit<T, M> ReverseDigit(Digit<T, M> d)
        {
            var l = new List<T>(d.Digits);
            l.Reverse();

            return new Digit<T, M>(d.M, l);
        }

        public static FingerTree<T, M> CreateR(Digit<T, M> left, FingerTree<Node<T, M>, M> f, List<T> right)
        {
            var m = left.M;

            if (right.Count > 0)
                return new DeepFingerTree<T, M>(m, left, f, new Digit<T, M>(m, right));

            if (f is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(left.Digits, m);

            var ir = f.RightView();
            var n = ir.End.Nodes;

            return new DeepFingerTree<T, M>(m, left, ir.Rest, new Digit<T, M>(m, n));
        }

        public static List<Node<T, M>> ListOfNodes(Monoid<M> m, List<T> tt)
        {
            var list = new List<Node<T, M>>();
            Node<T, M> next = null;

            if (tt.Count < 4)
            {
                next = new Node<T, M>(m, tt);
                list.Add(next);
                return list;
            }

            next = new Node<T, M>(m, new List<T>(tt.GetRange(0, 3)));
            list.Add(next);
            list.AddRange(ListOfNodes(m, tt.GetRange(3, tt.Count - 3)));
            return list;
        }

        public class Digit<U, V> : ISplittable<U, V> where U : IMeasured<V>
        {
            public readonly Monoid<V> M;
            public readonly List<U> Digits = new List<U>();

            public Digit(Monoid<V> m, U u1)
            {
                M = m;
                Digits.Add(u1);
            }

            public Digit(Monoid<V> m, U u1, U u2)
            {
                M = m;
                Digits.Add(u1);
                Digits.Add(u2);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3)
            {
                M = m;
                Digits.Add(u1);
                Digits.Add(u2);
                Digits.Add(u3);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3, U u4)
            {
                M = m;
                Digits.Add(u1);
                Digits.Add(u2);
                Digits.Add(u3);
                Digits.Add(u4);
            }

            public Digit(Monoid<V> m, List<U> uu)
            {
                M = m;
                Digits = uu;
            }

            public V Measure()
            {
                var result = M.Zero;

                foreach (U u in Digits)
                    result = M.BinaryOperator(result, u.Measure());

                return result;
            }

            public Split<Digit<U, V>, U, V> Split(Func<V, bool> p, V v)
            {
                int c = Digits.Count;
                if (c == 0)
                    throw new Exception("oops");

                var headItem = Digits[0];
                if (c == 1)
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, new Digit<U, V>(M, new List<U>()));

                var rest = new Digit<U, V>(M, new List<U>(Digits.GetRange(1, c - 1)));

                var a = M.BinaryOperator(v, headItem.Measure());
                if (p(a))
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, rest);

                var split = rest.Split(p, a);
                split.Left.Digits.Insert(0, headItem);
                return split;
            }

            public IEnumerable<U> ToSequenceLeft()
            {
                return Digits;
            }
        }

        public class Node<U, V> : IMeasured<V> where U : IMeasured<V>
        {
            public readonly Monoid<V> ConcreteMonoid;
            protected V PreCalcMeasure;
            public readonly List<U> Nodes = new List<U>();

            public Node(Monoid<V> m, U u1, U u2)
            {
                ConcreteMonoid = m;
                Nodes.Add(u1);
                Nodes.Add(u2);

                PreCalcMeasure = ConcreteMonoid.BinaryOperator(u1.Measure(), u2.Measure());
            }

            public Node(Monoid<V> m, U u1, U u2, U u3)
            {
                ConcreteMonoid = m;
                Nodes.Add(u1);
                Nodes.Add(u2);
                Nodes.Add(u3);

                PreCalcMeasure = ConcreteMonoid.Zero;
                foreach (var u in Nodes)
                    PreCalcMeasure = ConcreteMonoid.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public Node(Monoid<V> m, List<U> uu)
            {
                ConcreteMonoid = m;
                Nodes = uu;

                PreCalcMeasure = ConcreteMonoid.Zero;
                foreach (var u in Nodes)
                    PreCalcMeasure = ConcreteMonoid.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public V Measure()
            {
                return PreCalcMeasure;
            }
        }
    }
}
