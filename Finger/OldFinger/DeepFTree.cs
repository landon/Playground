using System;
using System.Collections.Generic;
using System.Text;

namespace FingerStandard
{
    public class DeepFTree<T> : FTree<T>
    {
        protected Digit<T> frontDig;
        protected FTree<Node<T>> innerFT;
        protected Digit<T> backDig;

        public DeepFTree(Digit<T> frontDig, FTree<Node<T>> innerFT, Digit<T> backDig)
        {
            if (frontDig.digNodes.Count > 0)
            {
                this.frontDig = frontDig;
                this.innerFT = innerFT;
                this.backDig = backDig;
            }
            else
            {
                throw new Exception("The DeepFTree() constructor is passed an empty frontDig !");
            }
        }

        public override ViewL<T> LeftView()
        {
            T head = frontDig.digNodes[0];

            var newFront = new List<T>(frontDig.digNodes);
            newFront.RemoveAt(0);

            return new ViewL<T>(head, FTree<T>.Create(newFront, innerFT, backDig));
        }

        public override ViewR<T> RightView()
        {
            int lastIndex = backDig.digNodes.Count - 1;
            T last = backDig.digNodes[lastIndex];

            List<T> newBack = new List<T>(backDig.digNodes);
            newBack.RemoveAt(lastIndex);

            return new ViewR<T>(FTree<T>.CreateR(frontDig, innerFT, newBack), last);
        }

        public override FTree<T> Push_Front(T t)
        {
            if (frontDig.digNodes.Count == 4)
            {
                var newFront = new List<T>(frontDig.digNodes);
                newFront.RemoveAt(0);

                return new DeepFTree<T>(new Digit<T>(t, frontDig.digNodes[0]), innerFT.Push_Front(new Node<T>(newFront)), backDig);
            }
            else
            {
                var newFront = new List<T>(frontDig.digNodes);
                newFront.Insert(0, t);

                return new DeepFTree<T>(new Digit<T>(newFront), innerFT, backDig);
            }
        }

        public override FTree<T> Push_Back(T t)
        {
            int cntbackDig = backDig.digNodes.Count;

            if (backDig.digNodes.Count == 4)
            {
                var newBack = new List<T>(backDig.digNodes);
                newBack.RemoveAt(cntbackDig - 1);

                return new DeepFTree<T>(frontDig, innerFT.Push_Back(new Node<T>(newBack)), new Digit<T>(backDig.digNodes[cntbackDig - 1], t));
            }
            else
            {
                var newBack = new List<T>(backDig.digNodes);
                newBack.Add(t);

                return new DeepFTree<T>(frontDig, innerFT, new Digit<T>(newBack));
            }
        }

        public override IEnumerable<T> ToSequence()
        {
            ViewL<T> lView = LeftView();

            yield return lView.head;

            foreach (T t in lView.ftTail.ToSequence())
                yield return t;
        }

        public override IEnumerable<T> ToSequenceR()
        {
            ViewR<T> rView = RightView();

            yield return rView.last;

            foreach (T t in rView.ftInit.ToSequenceR())
                yield return t;
        }

        public override FTree<T> App2(List<T> ts, FTree<T> rightFT)
        {
            if (!(rightFT is DeepFTree<T>))
            {
                FTree<T> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.Push_Back(t);
                }

                return (rightFT is EmptyFTree<T>)
                          ? resultFT
                          : resultFT.Push_Back(rightFT.LeftView().head);
            }
            else
            {
                var deepRight = rightFT as DeepFTree<T>;
                var cmbList = new List<T>(backDig.digNodes);

                cmbList.AddRange(ts);
                cmbList.AddRange(deepRight.frontDig.digNodes);

                return new DeepFTree<T>(frontDig, innerFT.App2(FTree<T>.ListOfNodes(cmbList), deepRight.innerFT), deepRight.backDig);
            }
        }

        public override FTree<T> Merge(FTree<T> rightFT)
        {
            return App2(new List<T>(), rightFT);
        }
    }
}
