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

        public static FingerTree<T, M> Create(List<T> frontList, FingerTree<Node<T, M>, M> innerFT, Digit<T, M> backDig)
        {
            var m = backDig.M;

            if (frontList.Count > 0)
                return new DeepFingerTree<T, M>(m, new Digit<T, M>(m, frontList), innerFT, backDig);

            if (innerFT is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(backDig.digitNodes, m);

            var innerLeft = innerFT.LeftView();
            var newlstFront = innerLeft.End.Nodes;

            return new DeepFingerTree<T, M>(m, new Digit<T, M>(m, newlstFront), innerLeft.Rest, backDig);
        }

        public virtual FingerTree<T, M> Reverse()
        {
            return this;
        }

        public static Digit<T, M> ReverseDigit(Digit<T, M> theDigit)
        {
            var newDigitList = new List<T>(theDigit.digitNodes);
            newDigitList.Reverse();

            return new Digit<T, M>(theDigit.M, newDigitList);
        }

        public static FingerTree<T, M> CreateR(Digit<T, M> frontDig, FingerTree<Node<T, M>, M> innerFT, List<T> backList)
        {
            var m = frontDig.M;

            if (backList.Count > 0)
                return new DeepFingerTree<T, M>(m, frontDig, innerFT, new Digit<T, M>(m, backList));

            if (innerFT is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(frontDig.digitNodes, m);

            var innerRight = innerFT.RightView();
            var newlstBack = innerRight.End.Nodes;

            return new DeepFingerTree<T, M>(m, frontDig, innerRight.Rest, new Digit<T, M>(m, newlstBack));
        }

        public static List<Node<T, M>> ListOfNodes(Monoid<M> m, List<T> tt)
        {
            var resultNodeList = new List<Node<T, M>>();
            Node<T, M> nextNode = null;

            if (tt.Count < 4)
            {
                nextNode = new Node<T, M>(m, tt);
                resultNodeList.Add(nextNode);
                return resultNodeList;
            }

            var nextTList = new List<T>(tt.GetRange(0, 3));
            nextNode = new Node<T, M>(m, nextTList);
            resultNodeList.Add(nextNode);
            resultNodeList.AddRange(ListOfNodes(m, tt.GetRange(3, tt.Count - 3)));
            return resultNodeList;
        }

        public class Digit<U, V> : ISplittable<U, V> where U : IMeasured<V>
        {
            public Monoid<V> M;
            public List<U> digitNodes = new List<U>();

            public Digit(Monoid<V> m, U u1)
            {
                M = m;
                digitNodes.Add(u1);
            }

            public Digit(Monoid<V> m, U u1, U u2)
            {
                M = m;

                digitNodes.Add(u1);
                digitNodes.Add(u2);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3)
            {
                M = m;

                digitNodes.Add(u1);
                digitNodes.Add(u2);
                digitNodes.Add(u3);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3, U u4)
            {
                M = m;

                digitNodes.Add(u1);
                digitNodes.Add(u2);
                digitNodes.Add(u3);
                digitNodes.Add(u4);
            }

            public Digit(Monoid<V> m, List<U> listU)
            {
                M = m;
                digitNodes = listU;
            }

            public V Measure()
            {
                var result = M.Zero;

                foreach (U u in digitNodes)
                    result = M.BinaryOperator(result, u.Measure());

                return result;
            }

            public Split<Digit<U, V>, U, V> Split(Func<V, bool> predicate, V acc)
            {
                int cnt = digitNodes.Count;
                if (cnt == 0)
                    throw new Exception("oops");

                var headItem = digitNodes[0];
                if (cnt == 1)
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, new Digit<U, V>(M, new List<U>()));

                List<U> digNodesTail = new List<U>(digitNodes.GetRange(1, cnt - 1));
                Digit<U, V> digitTail = new Digit<U, V>(M, digNodesTail);

                var acc1 = M.BinaryOperator(acc, headItem.Measure());
                if (predicate(acc1))
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, digitTail);

                var tailSplit = digitTail.Split(predicate, acc1);
                tailSplit.Left.digitNodes.Insert(0, headItem);
                return tailSplit;
            }

            public IEnumerable<U> ToSequenceLeft()
            {
                return digitNodes;
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
                foreach (U u in Nodes)
                    PreCalcMeasure = ConcreteMonoid.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public Node(Monoid<V> m, List<U> uu)
            {
                ConcreteMonoid = m;
                Nodes = uu;

                PreCalcMeasure = ConcreteMonoid.Zero;
                foreach (U u in Nodes)
                    PreCalcMeasure = ConcreteMonoid.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public V Measure()
            {
                return PreCalcMeasure;
            }
        }
    }
}
