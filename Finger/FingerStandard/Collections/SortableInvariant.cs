using System;

namespace FingerStandard.Collections
{
    public class SortableInvariant<T, V> where V : IComparable
    {
        public readonly V Degenerate;
        public readonly Func<T,V> Invariant;

        public SortableInvariant(V degenerate, Func<T, V> invariant)
        {
            Degenerate = degenerate;
            Invariant = invariant;
        }
    }
}
