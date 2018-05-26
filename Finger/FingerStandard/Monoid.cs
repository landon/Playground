using System;

namespace FingerStandard
{
    public class Monoid<T>
    {
        public readonly T Zero;
        public readonly Func<T, T, T> BinaryOperator;

        public Monoid(T zero, Func<T, T, T> bo)
        {
            Zero = zero;
            BinaryOperator = bo;
        }
    }
}
