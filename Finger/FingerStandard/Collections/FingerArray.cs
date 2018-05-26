using System;
using System.Collections;
using System.Collections.Generic;

namespace FingerStandard.Collections
{
    public class FingerArray<T> : FingerTree<SizedElement<T>, int>
    {
        static bool LessThan(int n, int i) => n < i;

        FingerTree<SizedElement<T>, int> _finger = new EmptyFingerTree<SizedElement<T>, int>(Monoids.Size);
        public int Length => _finger.Measure();

        public FingerArray(IEnumerable<T> tt)
        {
            foreach (T t in tt)
                _finger = _finger.PushRight(new SizedElement<T>(t));
        }

        public FingerArray(FingerTree<SizedElement<T>, int> f)
        {
            _finger = f;
        }

        public override Monoid<int> ConcreteMonoid => _finger.ConcreteMonoid;
        public override int Measure() => _finger.Measure();

        public override FingerTree<SizedElement<T>, int> PushLeft(SizedElement<T> t) => new FingerArray<T>(_finger.PushLeft(t));
        public override FingerTree<SizedElement<T>, int> PushRight(SizedElement<T> t) => new FingerArray<T>(_finger.PushRight(t));
        public override IEnumerable<SizedElement<T>> ToSequenceLeft() => _finger.ToSequenceLeft();
        public override IEnumerable<SizedElement<T>> ToSequenceRight() => _finger.ToSequenceRight();

        public override View<SizedElement<T>, int> LeftView()
        {
            var v = _finger.LeftView();
            v.Rest = new FingerArray<T>(v.Rest);
            return v;
        }

        public override View<SizedElement<T>, int> RightView()
        {
            var v = _finger.RightView();
            v.Rest = new FingerArray<T>(v.Rest);
            return v;
        }

        public override FingerTree<SizedElement<T>, int> Reverse() => new FingerArray<T>(_finger.Reverse());
        public override FingerTree<SizedElement<T>, int> Merge(FingerTree<SizedElement<T>, int> f) => _finger.Merge(f);

        public override Split<FingerTree<SizedElement<T>, int>, SizedElement<T>, int> Split(Func<int, bool> p, int a)
        {
            var split = _finger.Split(p, a);

            split.Left = new FingerArray<T>(split.Left);
            split.Right = new FingerArray<T>(split.Right);

            return split;
        }

        public override Pair<FingerTree<SizedElement<T>, int>, FingerTree<SizedElement<T>, int>> Split(Func<int, bool> p)
        {
            var split = _finger.Split(p);

            split.Left = new FingerArray<T>(split.Left);
            split.Right = new FingerArray<T>(split.Right);

            return split;
        }

        public override FingerTree<SizedElement<T>, int> App2(List<SizedElement<T>> ts, FingerTree<SizedElement<T>, int> f) => _finger.App2(ts, f);

        public Pair<FingerArray<T>, FingerArray<T>> SplitAt(int i)
        {
            var split = _finger.Split(((Func<int, int, bool>)LessThan).Curry(i));
            return new Pair<FingerArray<T>, FingerArray<T>>(new FingerArray<T>(split.Left), new FingerArray<T>(split.Right));
        }

        public T ElementAt(int i) => _finger.Split(((Func<int, int, bool>)LessThan).Curry(i), 0).SplitItem.E;
        public T this[int i] => ElementAt(i);

        public FingerArray<T> InsertAt(int i, T t)
        {
            if (0 > i || i > Length)
                throw new IndexOutOfRangeException(string.Format("oops", i, Length));

            var split = SplitAt(i);
            return new FingerArray<T>(split.Left.Merge(split.Right.PushLeft(new SizedElement<T>(t))));
        }

        public FingerArray<T> RemoveAt(int i)
        {
            if (0 > i || i > Length)
                throw new IndexOutOfRangeException(string.Format("oops", i, Length));

            var split = SplitAt(i);
            return new FingerArray<T>(split.Left._finger.Merge(split.Right._finger.LeftView().Rest));
        }
    }
}
