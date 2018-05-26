namespace FingerStandard.Collections
{
    public abstract class Element<T, V> : IMeasured<V>
    {
        protected readonly T _t;
        public T E => _t;

        public Element(T t)
        {
            _t = t;
        }

        public abstract V Measure();
    }
}
