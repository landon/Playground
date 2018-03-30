using System.Collections.Generic;

namespace FingerStandard
{
    public interface ISplittable<T, M> : IMeasured<M>
    {
        IEnumerable<T> ToSequence();
    }
}
