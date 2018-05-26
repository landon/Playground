using System;

namespace FingerStandard
{
    public static class Extensions
    {
        public static Func<Y, Z> Curry<X, Y, Z>(this Func<X, Y, Z> f, X x) => y => f(x, y);
    }
}
