using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public abstract class FingerTree<T, M> : ISplittable<T, M> where T : IMeasured<M>
    {
        public abstract Monoid<M> treeMonoid { get; }
        public abstract M Measure();

        public abstract FingerTree<T, M> Push_Front(T t);
        public abstract FingerTree<T, M> Push_Back(T t);

        public abstract IEnumerable<T> ToSequence();
        public abstract IEnumerable<T> ToSequenceR();

        public abstract View<T, M> LeftView();
        public abstract View<T, M> RightView();

        public abstract FingerTree<T, M> Merge(FingerTree<T, M> rightFT);
        public abstract FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> rightFT);

        public abstract Split<FingerTree<T, M>, T, M> Split(Func<M,bool> predicate, M acc);
        public abstract Pair<FingerTree<T, M>, FingerTree<T, M>> SeqSplit(Func<M, bool> predicate);

        public FingerTree<T, M> takeUntil(Func<M, bool> predicate) => SeqSplit(predicate).Left;
        public FingerTree<T, M> dropUntil(Func<M, bool> predicate) => SeqSplit(predicate).Right;
        public T Lookup(Func<M, bool> predicate, M acc) => dropUntil(predicate).LeftView().End;

        public static FingerTree<T, M> FromSequence(IEnumerable<T> sequence, Monoid<M> m)
        {
            var sequenceEnum = sequence.GetEnumerator();
            FingerTree<T, M> r = new EmptyFingerTree<T, M>(m);

            while (sequenceEnum.MoveNext())
                r = r.Push_Back(sequenceEnum.Current);

            return r;
        }

        public static FingerTree<T, M> Create(List<T> frontList, FingerTree<Node<T, M>, M> innerFT, Digit<T, M> backDig)
        {
            var m = backDig.M;

            if (frontList.Count > 0)
                return new DeepFingerTree<T, M>(m, new Digit<T, M>(m, frontList), innerFT, backDig);

            if (innerFT is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(backDig.digNodes, m);

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
            var newDigitList = new List<T>(theDigit.digNodes);
            newDigitList.Reverse();

            return new Digit<T, M>(theDigit.M, newDigitList);
        }

        public static FingerTree<T, M> CreateR(Digit<T, M> frontDig, FingerTree<Node<T, M>, M> innerFT, List<T> backList)
        {
            var m = frontDig.M;

            if (backList.Count > 0)
                return new DeepFingerTree<T, M>(m, frontDig, innerFT, new Digit<T, M>(m, backList));

            if (innerFT is EmptyFingerTree<Node<T, M>, M>)
                return FromSequence(frontDig.digNodes, m);

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
            public List<U> digNodes = new List<U>();

            public Digit(Monoid<V> m, U u1)
            {
                M = m;
                digNodes.Add(u1);
            }

            public Digit(Monoid<V> m, U u1, U u2)
            {
                M = m;

                digNodes.Add(u1);
                digNodes.Add(u2);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3)
            {
                M = m;

                digNodes.Add(u1);
                digNodes.Add(u2);
                digNodes.Add(u3);
            }
            public Digit(Monoid<V> m, U u1, U u2, U u3, U u4)
            {
                M = m;

                digNodes.Add(u1);
                digNodes.Add(u2);
                digNodes.Add(u3);
                digNodes.Add(u4);
            }

            public Digit(Monoid<V> m, List<U> listU)
            {
                M = m;
                digNodes = listU;
            }

            public V Measure()
            {
                var result = M.Zero;

                foreach (U u in digNodes)
                    result = M.BinaryOperator(result, u.Measure());

                return result;
            }

            public Split<Digit<U, V>, U, V> Split(Func<V, bool> predicate, V acc)
            {
                int cnt = digNodes.Count;
                if (cnt == 0)
                    throw new Exception("Error: Split of an empty Digit attempted!");

                var headItem = digNodes[0];
                if (cnt == 1)
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, new Digit<U, V>(M, new List<U>()));

                List<U> digNodesTail = new List<U>(digNodes.GetRange(1, cnt - 1));
                Digit<U, V> digitTail = new Digit<U, V>(M, digNodesTail);

                var acc1 = M.BinaryOperator(acc, headItem.Measure());
                if (predicate(acc1))
                    return new Split<Digit<U, V>, U, V>(new Digit<U, V>(M, new List<U>()), headItem, digitTail);

                var tailSplit = digitTail.Split(predicate, acc1);
                tailSplit.Left.digNodes.Insert(0, headItem);
                return tailSplit;
            }

            public IEnumerable<U> ToSequence()
            {
                return digNodes;
            }
        }

        public class Node<U, V> : IMeasured<V> where U : IMeasured<V>
        {
            public readonly Monoid<V> OM;
            protected V PreCalcMeasure;
            public readonly List<U> Nodes = new List<U>();

            public Node(Monoid<V> m, U u1, U u2)
            {
                OM = m;
                Nodes.Add(u1);
                Nodes.Add(u2);

                PreCalcMeasure = OM.BinaryOperator(u1.Measure(), u2.Measure());
            }

            public Node(Monoid<V> m, U u1, U u2, U u3)
            {
                OM = m;
                Nodes.Add(u1);
                Nodes.Add(u2);
                Nodes.Add(u3);

                PreCalcMeasure = OM.Zero;
                foreach (U u in Nodes)
                    PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public Node(Monoid<V> m, List<U> uu)
            {
                OM = m;
                Nodes = uu;

                PreCalcMeasure = OM.Zero;
                foreach (U u in Nodes)
                    PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, u.Measure());
            }

            public V Measure()
            {
                return PreCalcMeasure;
            }
        }
    }
}
