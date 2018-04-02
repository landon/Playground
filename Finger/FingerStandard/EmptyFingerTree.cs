using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public class EmptyFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        Monoid<M> _m;

        public EmptyFingerTree(Monoid<M> m)
        {
            _m = m;
        }

        public override Monoid<M> ConcreteMonoid => _m;
        public override M Measure() => _m.Zero;

        public override FingerTree<T, M> PushLeft(T t) => new SingleFingerTree<T, M>(_m, t);
        public override FingerTree<T, M> PushRight(T t) => new SingleFingerTree<T, M>(_m, t);
        public override IEnumerable<T> ToSequenceLeft() => new List<T>();
        public override IEnumerable<T> ToSequenceRight() => new List<T>();
        public override View<T, M> LeftView() => null;
        public override View<T, M> RightView() => null;

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> f)
        {
            for (int i = ts.Count - 1; i >= 0; i--)
                f = f.PushLeft(ts[i]);

            return f;
        }

        public override FingerTree<T, M> Merge(FingerTree<T, M> f) => f;
        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> p, M a) => throw new Exception("oops");
        public override Pair<FingerTree<T, M>, FingerTree<T, M>> Split(Func<M, bool> p) => new Pair<FingerTree<T, M>, FingerTree<T, M>>(new EmptyFingerTree<T, M>(_m), new EmptyFingerTree<T, M>(_m));
    }
}
