using System;
using System.Linq;
using System.Collections.Generic;

namespace FingerStandard
{
    public class DeepFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        public Monoid<M> OM;
        protected M PreCalcMeasure;
        protected Digit<T, M> _frontDigits;
        protected FingerTree<Node<T, M>, M> _fingerTree;
        protected Digit<T, M> _backDigits;

        public DeepFingerTree(Monoid<M> m, Digit<T, M> f, FingerTree<Node<T, M>, M> ft, Digit<T, M> b)
        {
            if (f.digNodes.Count > 0)
            {
                OM = m;

                _frontDigits = f;
                _fingerTree = ft;
                _backDigits = b;

                PreCalcMeasure = OM.Zero;
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, f.Measure());
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, ft.Measure());
                PreCalcMeasure = OM.BinaryOperator(PreCalcMeasure, b.Measure());
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
            var newFrontDig = ReverseDigit(_backDigits);
            var newBackDig = ReverseDigit(_frontDigits);

            if (_fingerTree is EmptyFingerTree<Node<T, M>, M>)
                return new DeepFingerTree<T, M>(OM, newFrontDig, _fingerTree, newBackDig);

            if (_fingerTree is SingleFingerTree<Node<T, M>, M>)
            {
                return new DeepFingerTree<T, M>(OM, newFrontDig, new SingleFingerTree<Node<T, M>, M>(OM, ReverseNode(_fingerTree.LeftView().End)), newBackDig);
            }

            var revDeepInner = (DeepFingerTree<Node<T, M>, M>)(((DeepFingerTree<Node<T, M>, M>)_fingerTree).Reverse());
            var newFrontNodes = new List<Node<T, M>>();
            var newBackNodes = new List<Node<T, M>>();

            foreach (var node in revDeepInner._frontDigits.digNodes)
                newFrontNodes.Add(ReverseNode(node));

            foreach (var node in revDeepInner._backDigits.digNodes)
                newBackNodes.Add(ReverseNode(node));

            var reversedInner = new DeepFingerTree<Node<T, M>, M>(OM, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(OM, newFrontNodes), revDeepInner._fingerTree, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(OM, newBackNodes));
            return new DeepFingerTree<T, M>(OM, ReverseDigit(_backDigits), reversedInner, ReverseDigit(_frontDigits));
        }

        static Node<T, M> ReverseNode(Node<T, M> n)
        {
            var theNodes = new List<T>(n.Nodes);
            theNodes.Reverse();

            return new Node<T, M>(n.OM, theNodes);
        }

        public override View<T, M> LeftView() => new View<T, M>(_frontDigits.digNodes[0], Create(_frontDigits.digNodes.Skip(1).ToList(), _fingerTree, _backDigits));
        public override View<T, M> RightView() => new View<T, M>(_backDigits.digNodes[_backDigits.digNodes.Count - 1], CreateR(_frontDigits, _fingerTree, _backDigits.digNodes.Take(_backDigits.digNodes.Count - 1).ToList()));

        public override FingerTree<T, M> Push_Front(T t)
        {
            if (_frontDigits.digNodes.Count == 4)
            {
                return new DeepFingerTree<T, M>(OM, new Digit<T, M>(OM, t, _frontDigits.digNodes[0]), _fingerTree.Push_Front(new Node<T, M>(OM, _frontDigits.digNodes.Skip(1).ToList())), _backDigits);
            }
            else
            {
                var newFront = new List<T>(_frontDigits.digNodes);
                newFront.Insert(0, t);

                return new DeepFingerTree<T, M>(OM, new Digit<T, M>(OM, newFront), _fingerTree, _backDigits);
            }
        }

        public override FingerTree<T, M> Push_Back(T t)
        {
            int cntbackDig = _backDigits.digNodes.Count;
            if (_backDigits.digNodes.Count == 4)
            {
                return new DeepFingerTree<T, M>(OM, _frontDigits, _fingerTree.Push_Back(new Node<T, M>(OM, _backDigits.digNodes.Take(3).ToList())), new Digit<T, M>(OM, _backDigits.digNodes[3], t));
            }
            else
            {
                var newBack = new List<T>(_backDigits.digNodes);
                newBack.Add(t);

                return new DeepFingerTree<T, M>(OM, _frontDigits, _fingerTree, new Digit<T, M>(OM, newBack));
            }
        }

        public override IEnumerable<T> ToSequence()
        {
            var view = LeftView();
            yield return view.End;
            foreach (T t in view.Rest.ToSequence())
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

                var cmbList = new List<T>(_backDigits.digNodes);
                cmbList.AddRange(ts);
                cmbList.AddRange(deepRight._frontDigits.digNodes);

                return new DeepFingerTree<T, M>(OM, _frontDigits, _fingerTree.App2(ListOfNodes(OM, cmbList), deepRight._fingerTree), deepRight._backDigits);
            }
        }


        public override FingerTree<T, M> Merge(FingerTree<T, M> rightFT)
        {
            return App2(new List<T>(), rightFT);
        }

        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> predicate, M acc)
        {
            var vPr = OM.BinaryOperator(acc, _frontDigits.Measure());

            if (predicate(vPr))
            {
                var frontSplit = _frontDigits.Split(predicate, acc);
                return new Split<FingerTree<T, M>, T, M>(FingerTree<T, M>.FromSequence(frontSplit.Left.digNodes, OM), frontSplit.SplitItem, FingerTree<T, M>.Create(frontSplit.Right.digNodes, _fingerTree, _backDigits));
            }

            var vM = OM.BinaryOperator(vPr, _fingerTree.Measure());

            if (predicate(vM))
            {
                var midSplit = _fingerTree.Split(predicate, vPr);
                var midLeft = midSplit.Left;
                var midItem = midSplit.SplitItem;
                var splitMidLeft = (new Digit<T, M>(OM, midItem.Nodes)).Split(predicate, OM.BinaryOperator(vPr, midLeft.Measure()));
                var finalsplitItem = splitMidLeft.SplitItem;
                var finalLeftTree = FingerTree<T, M>.CreateR(_frontDigits, midLeft, splitMidLeft.Left.digNodes);
                var finalRightTree = FingerTree<T, M>.Create(splitMidLeft.Right.digNodes, midSplit.Right, _backDigits);

                return new Split<FingerTree<T, M>, T, M>(finalLeftTree, finalsplitItem, finalRightTree);
            }

            var backSplit = _backDigits.Split(predicate, vM);
            return new Split<FingerTree<T, M>, T, M>(FingerTree<T, M>.CreateR(_frontDigits, _fingerTree, backSplit.Left.digNodes), backSplit.SplitItem, FingerTree<T, M>.FromSequence(backSplit.Right.digNodes, OM));
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
