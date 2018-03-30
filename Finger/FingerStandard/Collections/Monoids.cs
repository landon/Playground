using System;

namespace FingerStandard.Collections
{
    public static class Monoids
    {
        public static readonly Monoid<double> Priority = new Monoid<double>(double.NegativeInfinity, (a, b) => a > b ? a : b);
        public static readonly Monoid<int> Size = new Monoid<int>(0, (a, b) => a + b);
    }
}
