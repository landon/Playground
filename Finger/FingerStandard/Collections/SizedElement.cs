namespace FingerStandard.Collections
{
    public class SizedElement<T> : Element<T, int>
    {
        public SizedElement(T t) : base(t) { }
        public override int Measure() => 1;
    }
}
