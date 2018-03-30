namespace FingerStandard
{
    public class Split<U, T, V> where U : ISplittable<T, V> where T : IMeasured<V>
    {
        public readonly U Left;
        public readonly T SplitItem;
        public readonly U Right;

        public Split(U left, T splitItem, U right)
        { 
            Left = left;
            SplitItem = splitItem;
            Right = right;
        }
    }
}
