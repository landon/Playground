namespace FingerStandard.Collections
{
    public class WeightedElement<T> : Element<T, double>
    {
        readonly double _w;

        public WeightedElement(T t) : base(t)
        {
            _w = double.Parse(t.ToString());
        }

        public override double Measure() => _w;
    }
}
