using System;
using System.Collections.Generic;

namespace FingerStandard
{
    public partial class EmptyFingerTree<T, M> : FingerTree<T, M> where T : IMeasured<M>
    {
        Monoid<M> _m;

        public EmptyFingerTree(Monoid<M> m)
        {
            _m = m;
        }

        public override Monoid<M> treeMonoid => _m;
        public override M Measure() => _m.Zero;

        public override FingerTree<T, M> Push_Front(T t) => new SingleFingerTree<T, M>(_m, t);
        public override FingerTree<T, M> Push_Back(T t) => new SingleFingerTree<T, M>(_m, t);
        public override IEnumerable<T> ToSequence() => new List<T>();
        public override IEnumerable<T> ToSequenceR() => new List<T>();
        public override View<T, M> LeftView() => null;
        public override View<T, M> RightView() => null;

        public override FingerTree<T, M> App2(List<T> ts, FingerTree<T, M> rightFT)
        {
            var resultFT = rightFT;

            for (int i = ts.Count - 1; i >= 0; i--)
            {
                resultFT = resultFT.Push_Front(ts[i]);
            }

            return resultFT;
        }

        public override FingerTree<T, M> Merge(FingerTree<T, M> rightFT) => rightFT;
        public override Split<FingerTree<T, M>, T, M> Split(Func<M, bool> predicate, M acc) => throw new Exception("Error: Split attempted on an EmptyFTreeM !");
        public override Pair<FingerTree<T, M>, FingerTree<T, M>> SeqSplit(Func<M, bool> predicate) => new Pair<FingerTree<T, M>, FingerTree<T, M>>(new EmptyFingerTree<T, M>(_m), new EmptyFingerTree<T, M>(_m));
    }
}
