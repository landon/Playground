using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public class SingleFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        Monoid<M> _m;
        T _t;

        public SingleFingerTree(Monoid<M> m, T t)
        {
            _m = m;
            _t = t;
        }

        public override Monoid<M> ConcreteMonoid => _m;
        public override M Measure() => _t.Measure();
        public override FingerTree<T, M> PushLeft(T t) => new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, t), new EmptyFingerTree<Node<T, M>, M>(_m), new Digit<T, M>(_m, _t));
        public override FingerTree<T, M> PushRight(T t) => new DeepFingerTree<T, M>(_m, new Digit<T, M>(_m, _t), new EmptyFingerTree<Node<T, M>, M>(_m), new Digit<T, M>(_m, t));
        public override IEnumerable<T> ToSequenceLeft() => new[] { _t };
        public override IEnumerable<T> ToSequenceRight() => new[] { _t };
        public override View<T, M> LeftView() => new View<T, M>(_t, new EmptyFingerTree<T, M>(_m));
        public override View<T, M> RightView() => new View<T, M>(_t, new EmptyFingerTree<T, M>(_m));
        public override FingerTree<T, M> Merge(FingerTree<T, M> f) => f.PushLeft(_t);
        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> p, M a) => new Split<FingerTree<T, M>, T, M>(new EmptyFingerTree<T, M>(_m), _t, new EmptyFingerTree<T, M>(_m));

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> f)
        {
            for (int i = ts.Count - 1; i >= 0; i--)
                f = f.PushLeft(ts[i]);

            return f.PushLeft(_t);
        }

        public override Pair<FingerTree<T, M>, FingerTree<T, M>> Split(Func<M, bool> p) => p(_t.Measure()) ? new Pair<FingerTree<T, M>, FingerTree<T, M>>(new EmptyFingerTree<T, M>(_m), this) 
                                                                                                           : new Pair<FingerTree<T, M>, FingerTree<T, M>>(this, new EmptyFingerTree<T, M>(_m));
    }
}
