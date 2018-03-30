using System;
using System.Linq;
using System.Collections.Generic;

namespace FingerStandard
{
    public class DeepFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        Monoid<M> _m;
        M PreCalcMeasure;
        Digit<T, M> _leftDigits;
        FingerTree<Node<T, M>, M> _fingerTree;
        Digit<T, M> _rightDigits;

        public DeepFingerTree(Monoid<M> m, Digit<T, M> f, FingerTree<Node<T, M>, M> ft, Digit<T, M> b)
        {
            if (f.digitNodes.Count > 0)
            {
                _m = m;

                _leftDigits = f;
                _fingerTree = ft;
                _rightDigits = b;

                PreCalcMeasure = _m.Zero;
                PreCalcMeasure = _m.BinaryOperator(PreCalcMeasure, f.Measure());
                PreCalcMeasure = _m.BinaryOperator(PreCalcMeasure, ft.Measure());
                PreCalcMeasure = _m.BinaryOperator(PreCalcMeasure, b.Measure());
            }
            else
            {
                throw new Exception("The DeepFingerTree() constructor was passed an empty front digits deal.");
            }
        }

        public override Monoid<M> ConcreteMonoid => _m;

        public override M Measure()
        { 
            return PreCalcMeasure;
        }

        public override FingerTree<T, M> Reverse()
        {
            var newFrontDig = ReverseDigit(_rightDigits);
            var newBackDig = ReverseDigit(_leftDigits);

            if (_fingerTree is EmptyFingerTree<Node<T, M>, M>)
                return new DeepFingerTree<T, M>(_m, newFrontDig, _fingerTree, newBackDig);

            if (_fingerTree is SingleFingerTree<Node<T, M>, M>)
            {
                return new DeepFingerTree<T, M>(_m, newFrontDig, new SingleFingerTree<Node<T, M>, M>(_m, ReverseNode(_fingerTree.LeftView().End)), newBackDig);
            }

            var revDeepInner = (DeepFingerTree<Node<T, M>, M>)(((DeepFingerTree<Node<T, M>, M>)_fingerTree).Reverse());
            var newFrontNodes = new List<Node<T, M>>();
            var newBackNodes = new List<Node<T, M>>();

            foreach (var node in revDeepInner._leftDigits.digitNodes)
                newFrontNodes.Add(ReverseNode(node));

            foreach (var node in revDeepInner._rightDigits.digitNodes)
                newBackNodes.Add(ReverseNode(node));

            var reversedInner = new DeepFingerTree<Node<T, M>, M>(_m, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(_m, newFrontNodes), revDeepInner._fingerTree, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(_m, newBackNodes));
            return new DeepFingerTree<T, M>(_m, ReverseDigit(_rightDigits), reversedInner, ReverseDigit(_leftDigits));
        }

        public override View<T, M> LeftView() => new View<T, M>(_leftDigits.digitNodes[0], Create(_leftDigits.digitNodes.Skip(1).ToList(), _fingerTree, _rightDigits));
        public override View<T, M> RightView() => new View<T, M>(_rightDigits.digitNodes[_rightDigits.digitNodes.Count - 1], CreateR(_leftDigits, _fingerTree, _rightDigits.digitNodes.Take(_rightDigits.digitNodes.Count - 1).ToList()));

        public override FingerTree<T, M> PushLeft(T t)
        {
            if (_leftDigits.digitNodes.Count == 4)
            {
                return new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, t, _leftDigits.digitNodes[0]), _fingerTree.PushLeft(new Node<T, M>(_m, _leftDigits.digitNodes.Skip(1).ToList())), _rightDigits);
            }
            else
            {
                var newLeft = new List<T>(_leftDigits.digitNodes);
                newLeft.Insert(0, t);

                return new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, newLeft), _fingerTree, _rightDigits);
            }
        }

        public override FingerTree<T, M> PushRight(T t)
        {
            if (_rightDigits.digitNodes.Count == 4)
            {
                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree.PushRight(new Node<T, M>(_m, _rightDigits.digitNodes.Take(3).ToList())), new Digit<T, M>(_m, _rightDigits.digitNodes[3], t));
            }
            else
            {
                var newRight = new List<T>(_rightDigits.digitNodes);
                newRight.Add(t);

                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree, new Digit<T, M>(_m, newRight));
            }
        }

        public override IEnumerable<T> ToSequenceLeft()
        {
            var view = LeftView();
            yield return view.End;
            foreach (T t in view.Rest.ToSequenceLeft())
                yield return t;
        }

        public override IEnumerable<T> ToSequenceRight()
        {
            var view = RightView();
            yield return view.End;
            foreach (T t in view.Rest.ToSequenceRight())
                yield return t;
        }

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> f)
        {
            if (f is EmptyFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.PushRight(t);
                }

                return resultFT;
            }
            else if (f is SingleFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (T t in ts)
                {
                    resultFT = resultFT.PushRight(t);
                }

                return resultFT.PushRight(f.LeftView().End);
            }
            else
            {
                var deepRight = f as DeepFingerTree<T, M>;

                var ll = new List<T>(_rightDigits.digitNodes);
                ll.AddRange(ts);
                ll.AddRange(deepRight._leftDigits.digitNodes);

                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree.App2(ListOfNodes(_m, ll), deepRight._fingerTree), deepRight._rightDigits);
            }
        }


        public override FingerTree<T, M> Merge(FingerTree<T, M> rightFT)
        {
            return App2(new List<T>(), rightFT);
        }

        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> p, M a)
        {
            var fdm = _m.BinaryOperator(a, _leftDigits.Measure());

            if (p(fdm))
            {
                var frontSplit = _leftDigits.Split(p, a);
                return new Split<FingerTree<T, M>, T, M>(FromSequence(frontSplit.Left.digitNodes, _m), frontSplit.SplitItem, Create(frontSplit.Right.digitNodes, _fingerTree, _rightDigits));
            }

            var ftm = _m.BinaryOperator(fdm, _fingerTree.Measure());

            if (p(ftm))
            {
                var midSplit = _fingerTree.Split(p, fdm);
                var midLeft = midSplit.Left;
                var midItem = midSplit.SplitItem;
                var splitMidLeft = (new Digit<T, M>(_m, midItem.Nodes)).Split(p, _m.BinaryOperator(fdm, midLeft.Measure()));
                var finalsplitItem = splitMidLeft.SplitItem;
                var finalLeftTree = CreateR(_leftDigits, midLeft, splitMidLeft.Left.digitNodes);
                var finalRightTree = Create(splitMidLeft.Right.digitNodes, midSplit.Right, _rightDigits);

                return new Split<FingerTree<T, M>, T, M>(finalLeftTree, finalsplitItem, finalRightTree);
            }

            var backSplit = _rightDigits.Split(p, ftm);
            return new Split<FingerTree<T, M>, T, M>(CreateR(_leftDigits, _fingerTree, backSplit.Left.digitNodes), backSplit.SplitItem, FromSequence(backSplit.Right.digitNodes, _m));
        }

        public override Pair<FingerTree<T, M>, FingerTree<T, M>> Split(Func<M, bool> p)
        {
            if (!p(Measure()))
                return new Pair<FingerTree<T, M>, FingerTree<T, M>>(this, new EmptyFingerTree<T, M>(_m));

            var theSplit = Split(p, _m.Zero);
            return new Pair<FingerTree<T, M>, FingerTree<T, M>>(theSplit.Left, theSplit.Right.PushLeft(theSplit.SplitItem));
        }

        static Node<T, M> ReverseNode(Node<T, M> n)
        {
            var theNodes = new List<T>(n.Nodes);
            theNodes.Reverse();

            return new Node<T, M>(n.ConcreteMonoid, theNodes);
        }
    }
}
