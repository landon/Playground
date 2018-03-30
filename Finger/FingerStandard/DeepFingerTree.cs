using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public partial class DeepFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        public Monoid<M> OM;
        protected M PreCalcMeasure;
        protected Digit<T, M> frontDig;
        protected FingerTree<Node<T, M>, M> innerFT;
        protected Digit<T, M> backDig;

        public DeepFingerTree(Monoid<M> m, Digit<T, M> frontDig, FingerTree<Node<T, M>, M> innerFT, Digit<T, M> backDig)
        {
            if (frontDig.digNodes.Count > 0)
            {
                OM = m;

                this.frontDig = frontDig;
                this.innerFT = innerFT;
                this.backDig = backDig;

                PreCalcMeasure = OM.Zero;
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, frontDig.Measure());
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, innerFT.Measure());
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, backDig.Measure());
            }
            else
            {
                throw new Exception("The DeepFTree() constructor is passed an empty frontDig !");
            }
        }

        public override Monoid<M> treeMonoid
        {
            get { return OM; }
        }

        public override M Measure()
        {
            return PreCalcMeasure;
        }

        public override FingerTree<T, M> Reverse()
        {
            Digit<T, M> newFrontDig = ReverseDigit(backDig);
            Digit<T, M> newBackDig = ReverseDigit(frontDig);

            if (innerFT is EmptyFingerTree<Node<T, M>, M>)
                return new DeepFingerTree<T, M>(OM, newFrontDig, innerFT, newBackDig);

            if (innerFT is SingleFingerTree<Node<T, M>, M>)
            {
                return new DeepFingerTree<T, M>(OM, newFrontDig, new SingleFingerTree<Node<T, M>, M>(OM, ReverseNode(innerFT.LeftView().End)), newBackDig);
            }

            var revDeepInner = (DeepFingerTree<Node<T, M>, M>)(((DeepFingerTree<Node<T, M>, M>)innerFT).Reverse());
            var newFrontNodes = new List<Node<T, M>>();
            var newBackNodes = new List<Node<T, M>>();

            foreach (var node in revDeepInner.frontDig.digNodes)
                newFrontNodes.Add(ReverseNode(node));

            foreach (var node in revDeepInner.backDig.digNodes)
                newBackNodes.Add(ReverseNode(node));

            var reversedInner = new DeepFingerTree<Node<T, M>, M>(OM, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(OM, newFrontNodes), revDeepInner.innerFT, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(OM, newBackNodes));
            return new DeepFingerTree<T, M>(OM, ReverseDigit(backDig), reversedInner, ReverseDigit(frontDig));
        }

        static Node<T, M> ReverseNode(Node<T, M> n)
        {
            var theNodes = new List<T>(n.Nodes);
            theNodes.Reverse();

            return new Node<T, M>(n.OM, theNodes);
        }

        public override View<T, M> LeftView()
        {
            T head = frontDig.digNodes[0];

            List<T> newFront = new List<T>(frontDig.digNodes);
            newFront.RemoveAt(0);
            return new View<T, M>(head, FingerTree<T, M>.Create(newFront, innerFT, backDig));
        }

        public override View<T, M> RightView()
        {
            int lastIndex = backDig.digNodes.Count - 1;
            T last = backDig.digNodes[lastIndex];

            List<T> newBack = new List<T>(backDig.digNodes);
            newBack.RemoveAt(lastIndex);

            return new View<T, M>(last, FingerTree<T, M>.CreateR(frontDig, innerFT, newBack));
        }

        public override FingerTree<T, M> Push_Front(T t)
        {
            if (frontDig.digNodes.Count == 4)
            {
                List<T> newFront = new List<T>(frontDig.digNodes);
                newFront.RemoveAt(0);

                return new DeepFingerTree<T, M>(OM, new Digit<T, M>(OM, t, frontDig.digNodes[0]), innerFT.Push_Front(new Node<T, M>(OM, newFront)), backDig);
            }
            else
            {
                List<T> newFront = new List<T>(frontDig.digNodes);
                newFront.Insert(0, t);

                return new DeepFingerTree<T, M>(OM, new Digit<T, M>(OM, newFront), innerFT, backDig);
            }
        }

        public override FingerTree<T, M> Push_Back(T t)
        {
            int cntbackDig = backDig.digNodes.Count;
            if (backDig.digNodes.Count == 4)
            {
                List<T> newBack = new List<T>(backDig.digNodes);
                newBack.RemoveAt(cntbackDig - 1);

                return new DeepFingerTree<T, M>
                    (OM,
                     frontDig,
                     innerFT.Push_Back(new Node<T, M>(OM, newBack)),
                     new Digit<T, M>(OM, backDig.digNodes[cntbackDig - 1], t));
            }
            else
            {
                List<T> newBack = new List<T>(backDig.digNodes);
                newBack.Add(t);

                return new DeepFingerTree<T, M>(OM, frontDig, innerFT, new Digit<T, M>(OM, newBack));
            }
        }

        public override IEnumerable<T> ToSequence()
        {
            var lView = LeftView();

            yield return lView.End;
            foreach (T t in lView.Rest.ToSequence())
                yield return t;
        }

        public override IEnumerable<T> ToSequenceR()
        {
            var view = RightView();

            yield return view.End;

            foreach (T t in view.Rest.ToSequenceR())
                yield return t;
        }

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> rightFT)
        {
            if (rightFT is EmptyFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.Push_Back(t);
                }

                return resultFT;

            }

            else if (rightFT is SingleFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.Push_Back(t);
                }

                return resultFT.Push_Back(rightFT.LeftView().End);
            }
            else
            {
                var deepRight = rightFT as DeepFingerTree<T, M>;

                var cmbList = new List<T>(backDig.digNodes);
                cmbList.AddRange(ts);
                cmbList.AddRange(deepRight.frontDig.digNodes);

                return new DeepFingerTree<T, M>(OM, frontDig, innerFT.App2(FingerTree<T, M>.ListOfNodes(OM, cmbList), deepRight.innerFT), deepRight.backDig);
            }
        }


        public override FingerTree<T, M> Merge(FingerTree<T, M> rightFT)
        {
            return App2(new List<T>(), rightFT);
        }

        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> predicate, M acc)
        {
            var vPr = OM.BinaryOperator(acc, frontDig.Measure());

            if (predicate(vPr))
            {
                var frontSplit = frontDig.Split(predicate, acc);
                return new Split<FingerTree<T, M>, T, M>(FingerTree<T, M>.FromSequence(frontSplit.Left.digNodes, OM), frontSplit.SplitItem, FingerTree<T, M>.Create(frontSplit.Right.digNodes, innerFT, backDig));
            }

            var vM = OM.BinaryOperator(vPr, innerFT.Measure());

            if (predicate(vM))
            {
                var midSplit = innerFT.Split(predicate, vPr);
                var midLeft = midSplit.Left;
                var midItem = midSplit.SplitItem;
                var splitMidLeft = (new Digit<T, M>(OM, midItem.Nodes)).Split(predicate, OM.BinaryOperator(vPr, midLeft.Measure()));
                var finalsplitItem = splitMidLeft.SplitItem;
                var finalLeftTree = FingerTree<T, M>.CreateR(frontDig, midLeft, splitMidLeft.Left.digNodes);
                var finalRightTree = FingerTree<T, M>.Create(splitMidLeft.Right.digNodes, midSplit.Right, backDig);

                return new Split<FingerTree<T, M>, T, M>(finalLeftTree, finalsplitItem, finalRightTree);
            }

            var backSplit = backDig.Split(predicate, vM);
            return new Split<FingerTree<T, M>, T, M>(FingerTree<T, M>.CreateR(frontDig, innerFT, backSplit.Left.digNodes), backSplit.SplitItem, FingerTree<T, M>.FromSequence(backSplit.Right.digNodes, OM));
        }

        public override Pair<FingerTree<T, M>, FingerTree<T, M>> SeqSplit(Func<M, bool> predicate)
        {
            if (!predicate(Measure()))
                return new Pair<FingerTree<T, M>, FingerTree<T, M>>(this, new EmptyFingerTree<T, M>(OM));

            var theSplit = Split(predicate, OM.Zero);
            return new Pair<FingerTree<T, M>, FingerTree<T, M>>(theSplit.Left, theSplit.Right.Push_Front(theSplit.SplitItem));
        }
    }
}
