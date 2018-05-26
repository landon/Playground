namespace FingerStandard
{
    public class View<X, Y> where X : IMeasured<Y>
    {
        public readonly X End;
        public FingerTree<X, Y> Rest;

        public View(X end, FingerTree<X, Y> rest)
        {
            End = end;
            Rest = rest;
        }
    }
}
