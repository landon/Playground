using System;

namespace FingerStandard.Collections
{
    public class OrderedElement<T, V> : Element<T, V> where V : IComparable
    {
        public readonly SortableInvariant<T, V> X;

        public OrderedElement(T t, SortableInvariant<T, V> x) : base(t)
        {
            X = x;
        }

        public override V Measure() => X.Invariant(E);
    }
}
