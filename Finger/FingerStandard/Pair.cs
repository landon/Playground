namespace FingerStandard
{
    public class Pair<T, V>
    {
        public readonly T Left;
        public readonly V Right;

        public Pair(T left, V right)
        {
            Left = left;
            Right = right;
        }
    }
}
