using System;
using System.Collections.Generic;
using System.Text;

namespace FingerStandard
{
    public abstract class FTree<T>
    {
        public abstract FTree<T> Push_Front(T t);
        public abstract FTree<T> Push_Back(T t);

        public abstract IEnumerable<T> ToSequence();
        public abstract IEnumerable<T> ToSequenceR();

        public abstract ViewL<T> LeftView();
        public abstract ViewR<T> RightView();

        public abstract FTree<T> Merge(FTree<T> rightFT);
        public abstract FTree<T> App2(List<T> ts, FTree<T> rightFT);

        public static FTree<T> Create(List<T> frontList, FTree<Node<T>> innerFT, Digit<T> backDig)
        {
            if (frontList.Count > 0)
                return new DeepFTree<T>(new Digit<T>(frontList), innerFT, backDig);

            if (innerFT is EmptyFTree<Node<T>>)
                return FromSequence(backDig.digNodes);

            var innerLeft = innerFT.LeftView();
            var newlstFront = innerLeft.head.theNodes;

            return new DeepFTree<T>(new Digit<T>(newlstFront), innerLeft.ftTail, backDig);
        }

        public static FTree<T> CreateR(Digit<T> frontDig, FTree<Node<T>> innerFT, List<T> backList)
        {
            if (backList.Count > 0)
                return new DeepFTree<T>(frontDig, innerFT, new Digit<T>(backList));

            if (innerFT is EmptyFTree<Node<T>>)
                return FromSequence(frontDig.digNodes);

            var innerRight = innerFT.RightView();
            var newlstBack = innerRight.last.theNodes;

            return new DeepFTree<T>(frontDig, innerRight.ftInit, new Digit<T>(newlstBack));
        }

        public static FTree<T> FromSequence(IEnumerable<T> sequence)
        {
            var sequenceEnum = sequence.GetEnumerator();

            FTree<T> ftResult = new EmptyFTree<T>();

            while (sequenceEnum.MoveNext())
            {
                ftResult = ftResult.Push_Back(sequenceEnum.Current);
            }

            return ftResult;
        }

        public static List<Node<T>> ListOfNodes(List<T> tList)
        {
            var resultNodeList = new List<Node<T>>();

            Node<T> nextNode = null;

            int tCount = tList.Count;

            if (tCount < 4)
            {
                nextNode = new Node<T>(tList);
                resultNodeList.Add(nextNode);
                return resultNodeList;
            }

            var nextTList = new List<T>(tList.GetRange(0, 3));

            nextNode = new Node<T>(nextTList);
            resultNodeList.Add(nextNode);
            resultNodeList.AddRange(ListOfNodes(tList.GetRange(3, tCount - 3)));

            return resultNodeList;
        }

        public class ViewL<X>
        {
            public X head;
            public FTree<X> ftTail;

            public ViewL(X head, FTree<X> ftTail)
            {
                this.head = head;
                this.ftTail = ftTail;
            }
        }

        public class ViewR<X>
        {
            public X last;
            public FTree<X> ftInit;

            public ViewR(FTree<X> ftInit, X last)
            {
                this.ftInit = ftInit;
                this.last = last;
            }
        }

        public class Digit<U>
        {
            public List<U> digNodes = new List<U>(); 

            public Digit(U u1)
            {
                digNodes.Add(u1);
            }

            public Digit(U u1, U u2)
            {
                digNodes.Add(u1);
                digNodes.Add(u2);
            }
            public Digit(U u1, U u2, U u3)
            {
                digNodes.Add(u1);
                digNodes.Add(u2);
                digNodes.Add(u3);
            }
            public Digit(U u1, U u2, U u3, U u4)
            {
                digNodes.Add(u1);
                digNodes.Add(u2);
                digNodes.Add(u3);
                digNodes.Add(u4);
            }

            public Digit(List<U> listU)
            {
                digNodes = listU;
            }
        }

        public class Node<V>
        {
            public List<V> theNodes = new List<V>();

            public Node(V v1, V v2)
            {
                theNodes.Add(v1);
                theNodes.Add(v2);
            }

            public Node(V v1, V v2, V v3)
            {
                theNodes.Add(v1);
                theNodes.Add(v2);
                theNodes.Add(v3);
            }

            public Node(List<V> listV)
            {
                theNodes = listV;
            }
        }
    }
}
