using System;
using System.Collections.Generic;

namespace FingerStandard.Collections
{
    public class PriorityQueue<T> : FingerTree<WeightedElement<T>, double>
    {
        static bool LessThanOrEqual(double a, double b) => a <= b;
        FingerTree<WeightedElement<T>, double> _finger = new EmptyFingerTree<WeightedElement<T>, double>(Monoids.Priority);

        public PriorityQueue(IEnumerable<T> tt)
        {
            foreach (var t in tt)
                _finger = _finger.PushRight(new WeightedElement<T>(t));
        }

        public PriorityQueue(FingerTree<WeightedElement<T>, double> elemTree)
        {
            _finger = elemTree;
        }

        public override Monoid<double> ConcreteMonoid => _finger.ConcreteMonoid;
        public override double Measure() => _finger.Measure();
        public override FingerTree<WeightedElement<T>, double> PushLeft(WeightedElement<T> elemT) => new PriorityQueue<T>(_finger.PushLeft(elemT));
        public override FingerTree<WeightedElement<T>, double> PushRight(WeightedElement<T> elemT) => new PriorityQueue<T>(_finger.PushRight(elemT));
        public override IEnumerable<WeightedElement<T>> ToSequenceLeft() => _finger.ToSequenceLeft();
        public override IEnumerable<WeightedElement<T>> ToSequenceRight() => _finger.ToSequenceRight();

        public override View<WeightedElement<T>, double> LeftView()
        {
            var internLView = _finger.LeftView();
            internLView.Rest = new PriorityQueue<T>(internLView.Rest);
            return internLView;
        }

        public override View<WeightedElement<T>, double> RightView()
        {
            var internRView = _finger.RightView();
            internRView.Rest = new PriorityQueue<T>(internRView.Rest);
            return internRView;
        }

        public override FingerTree<WeightedElement<T>, double> Merge(FingerTree<WeightedElement<T>, double> f)
        {
            if (!(f is PriorityQueue<T>))
                throw new Exception("Error: PriQue merge with non-PriQue attempted!");
  
            return new PriorityQueue<T>(_finger.Merge(((PriorityQueue<T>)f)._finger));
        }

        public override Split<FingerTree<WeightedElement<T>, double>, WeightedElement<T>, double> Split(Func<double, bool> p, double a)
        {
            var split = _finger.Split(p, a);

            split.Left = new PriorityQueue<T>(split.Left);
            split.Right = new PriorityQueue<T>(split.Right);

            return split;
        }

        public override Pair<FingerTree<WeightedElement<T>, double>, FingerTree<WeightedElement<T>, double>> Split(Func<double, bool> p)
        {
            var pair = _finger.Split(p);

            pair.Left = new PriorityQueue<T>(pair.Left);
            pair.Right = new PriorityQueue<T>(pair.Right);

            return pair;
        }

        public override FingerTree<WeightedElement<T>, double> App2(List<WeightedElement<T>> ts, FingerTree<WeightedElement<T>, double> f)
        {
            return _finger.App2(ts, f);
        }

        public Pair<T, PriorityQueue<T>> ExtractMax()
        {
            var split = _finger.Split(((Func<double, double, bool>)LessThanOrEqual).Curry(_finger.Measure()), Monoids.Priority.Zero);
            return new Pair<T, PriorityQueue<T>>(split.SplitItem.E, new PriorityQueue<T>(split.Left.Merge(split.Right)));
        }
    }
}
