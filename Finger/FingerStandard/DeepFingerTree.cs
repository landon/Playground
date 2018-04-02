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
            if (f.Digits.Count > 0)
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
                throw new Exception("oops");
            }
        }

        public override Monoid<M> ConcreteMonoid => _m;

        public override M Measure()
        { 
            return PreCalcMeasure;
        }

        public override FingerTree<T, M> Reverse()
        {
            var fl = ReverseDigit(_rightDigits);
            var fr = ReverseDigit(_leftDigits);

            if (_fingerTree is EmptyFingerTree<Node<T, M>, M>)
                return new DeepFingerTree<T, M>(_m, fl, _fingerTree, fr);

            if (_fingerTree is SingleFingerTree<Node<T, M>, M>)
                return new DeepFingerTree<T, M>(_m, fl, new SingleFingerTree<Node<T, M>, M>(_m, ReverseNode(_fingerTree.LeftView().End)), fr);

            var rdi = (DeepFingerTree<Node<T, M>, M>)(((DeepFingerTree<Node<T, M>, M>)_fingerTree).Reverse());
            var ln = new List<Node<T, M>>();
            var rn = new List<Node<T, M>>();

            foreach (var n in rdi._leftDigits.Digits)
                ln.Add(ReverseNode(n));

            foreach (var n in rdi._rightDigits.Digits)
                rn.Add(ReverseNode(n));

            var ri = new DeepFingerTree<Node<T, M>, M>(_m, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(_m, ln), rdi._fingerTree, new DeepFingerTree<Node<T, M>, M>.Digit<Node<T, M>, M>(_m, rn));
            return new DeepFingerTree<T, M>(_m, ReverseDigit(_rightDigits), ri, ReverseDigit(_leftDigits));
        }

        public override View<T, M> LeftView() => new View<T, M>(_leftDigits.Digits[0], Create(_leftDigits.Digits.Skip(1).ToList(), _fingerTree, _rightDigits));
        public override View<T, M> RightView() => new View<T, M>(_rightDigits.Digits[_rightDigits.Digits.Count - 1], CreateR(_leftDigits, _fingerTree, _rightDigits.Digits.Take(_rightDigits.Digits.Count - 1).ToList()));

        public override FingerTree<T, M> PushLeft(T t)
        {
            if (_leftDigits.Digits.Count == 4)
            {
                return new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, t, _leftDigits.Digits[0]), _fingerTree.PushLeft(new Node<T, M>(_m, _leftDigits.Digits.Skip(1).ToList())), _rightDigits);
            }
            else
            {
                var nl = new List<T>(_leftDigits.Digits);
                nl.Insert(0, t);

                return new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, nl), _fingerTree, _rightDigits);
            }
        }

        public override FingerTree<T, M> PushRight(T t)
        {
            if (_rightDigits.Digits.Count == 4)
            {
                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree.PushRight(new Node<T, M>(_m, _rightDigits.Digits.Take(3).ToList())), new Digit<T, M>(_m, _rightDigits.Digits[3], t));
            }
            else
            {
                var nr = new List<T>(_rightDigits.Digits);
                nr.Add(t);

                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree, new Digit<T, M>(_m, nr));
            }
        }

        public override IEnumerable<T> ToSequenceLeft()
        {
            var view = LeftView();
            yield return view.End;
            foreach (var t in view.Rest.ToSequenceLeft())
                yield return t;
        }

        public override IEnumerable<T> ToSequenceRight()
        {
            var view = RightView();
            yield return view.End;
            foreach (var t in view.Rest.ToSequenceRight())
                yield return t;
        }

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> f)
        {
            if (f is EmptyFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (var t in ts)
                {
                    resultFT = resultFT.PushRight(t);
                }

                return resultFT;
            }
            else if (f is SingleFingerTree<T, M>)
            {
                FingerTree<T, M> resultFT = this;

                foreach (var t in ts)
                {
                    resultFT = resultFT.PushRight(t);
                }

                return resultFT.PushRight(f.LeftView().End);
            }
            else
            {
                var dr = f as DeepFingerTree<T, M>;

                var ll = new List<T>(_rightDigits.Digits);
                ll.AddRange(ts);
                ll.AddRange(dr._leftDigits.Digits);

                return new DeepFingerTree<T, M>(_m, _leftDigits, _fingerTree.App2(ListOfNodes(_m, ll), dr._fingerTree), dr._rightDigits);
            }
        }


        public override FingerTree<T, M> Merge(FingerTree<T, M> f)
        {
            return App2(new List<T>(), f);
        }

        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> p, M a)
        {
            var fdm = _m.BinaryOperator(a, _leftDigits.Measure());

            if (p(fdm))
            {
                var lsplit = _leftDigits.Split(p, a);
                return new Split<FingerTree<T, M>, T, M>(FromSequence(lsplit.Left.Digits, _m), lsplit.SplitItem, Create(lsplit.Right.Digits, _fingerTree, _rightDigits));
            }

            var ftm = _m.BinaryOperator(fdm, _fingerTree.Measure());

            if (p(ftm))
            {
                var ms = _fingerTree.Split(p, fdm);
                var ml = ms.Left;
                var mi = ms.SplitItem;
                var sml = (new Digit<T, M>(_m, mi.Nodes)).Split(p, _m.BinaryOperator(fdm, ml.Measure()));
                var fi = sml.SplitItem;
                var flt = CreateR(_leftDigits, ml, sml.Left.Digits);
                var frt = Create(sml.Right.Digits, ms.Right, _rightDigits);

                return new Split<FingerTree<T, M>, T, M>(flt, fi, frt);
            }

            var rsplit = _rightDigits.Split(p, ftm);
            return new Split<FingerTree<T, M>, T, M>(CreateR(_leftDigits, _fingerTree, rsplit.Left.Digits), rsplit.SplitItem, FromSequence(rsplit.Right.Digits, _m));
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
