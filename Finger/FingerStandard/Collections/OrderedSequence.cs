using System;
using System.Linq;
using System.Collections.Generic;

namespace FingerStandard.Collections
{
    public class OrderedSeqeuence<T> : OrderedSequence<T, T> where T : IComparable
    {
        public OrderedSeqeuence() : base(t => t) { }
        public OrderedSeqeuence(IEnumerable<T> tt) : base(t => t, tt) { }
    }

    public class OrderedSequence<T, V> where V : IComparable
    {
        FingerTree<OrderedElement<T, V>, V> _finger;
        SortableInvariant<T, V> _x;

        public OrderedSequence(SortableInvariant<T, V> x)
        {
            _finger = new EmptyFingerTree<OrderedElement<T, V>, V>(new Monoid<V>(x.Degenerate, (a, b) => b.CompareTo(x.Degenerate) == 0 ? a : b));
            _x = x;
        }

        public OrderedSequence(SortableInvariant<T, V> x, IEnumerable<T> tt)
            : this(x)
        {
            var initial = new OrderedSequence<T, V>(x);
            foreach (T t in tt)
                initial = initial.PushRight(new OrderedElement<T, V>(t, x));

            _finger = initial._finger;
        }

        public OrderedSequence(Func<T, V> orderBy, V degenerate = default(V)) : this(new SortableInvariant<T, V>(degenerate, orderBy)) {}
        public OrderedSequence(Func<T, V> orderBy, IEnumerable<T> tt, V degenerate = default(V)) : this(new SortableInvariant<T, V>(degenerate, orderBy), tt) { }

        protected OrderedSequence(SortableInvariant<T, V> x, FingerTree<OrderedElement<T, V>, V> finger)
        {
            _x = x;
            _finger = finger;
        }

        public Pair<OrderedSequence<T, V>, OrderedSequence<T, V>> Partition(V v)
        {
            var split = _finger.Split(((Func<V,V,bool>)LessThanOrEqual).Curry(v));
            return new Pair<OrderedSequence<T, V>, OrderedSequence<T, V>>(new OrderedSequence<T, V>(_x, split.Left), new OrderedSequence<T, V>(_x, split.Right));
        }

        public OrderedSequence<T, V> Insert(T t)
        {
            var part = Partition(_x.Invariant(t));
            return new OrderedSequence<T, V>(_x, part.Left._finger.Merge(part.Right._finger.PushLeft(new OrderedElement<T, V>(t, _x))));
        }

        public OrderedSequence<T, V> DeleteAll(T t)
        {
            var v = _x.Invariant(t);
            var part = Partition(v);
            var lastTreeSplit = part.Right._finger.Split(((Func<V, V, bool>)LessThan).Curry(v));

            return new OrderedSequence<T, V>(_x, part.Left._finger.Merge(lastTreeSplit.Right));
        }

        static bool LessThan(V v1, V v2) => v1.CompareTo(v2) < 0;
        static bool LessThanOrEqual(V v1, V v2) => v1.CompareTo(v2) <= 0;

        public OrderedSequence<T, V> Merge(OrderedSequence<T, V> o) => new OrderedSequence<T, V>(_x, Merge(_finger, o._finger));
        public IEnumerable<T> ToSequence() => _finger.ToSequenceLeft().Select(e => e.E);

        OrderedSequence<T, V> PushRight(OrderedElement<T, V> o)
        {
            var v = _finger.RightView();

            if (v != null)
            {
                if (v.End.Measure().CompareTo(o.Measure()) > 0)
                    throw new Exception("OrderedSequence Error: PushRight() of an element less than the biggest seq el.");
            }

            return new OrderedSequence<T, V>(_x, _finger.PushRight(o));
        }

        OrderedSequence<T, V> PushLeft(OrderedElement<T, V> o)
        {
            var v = _finger.LeftView();

            if (v != null)
            {
                if (v.End.Measure().CompareTo(o.Measure()) < 0)
                    throw new Exception("OrderedSequence Error: PushLeft() of an element greater than the smallest seq el.");
            }

            return new OrderedSequence<T, V>(_x, _finger.PushLeft(o));
        }

        static FingerTree<OrderedElement<T, V>, V> Merge(FingerTree<OrderedElement<T, V>, V> f, FingerTree<OrderedElement<T, V>, V> g)
        {
            var v = g.LeftView();

            if (v == null)
                return f;

            var end = v.End;
            var rest = v.Rest;
            
            var fsplit = f.Split(((Func<V, V, bool>)LessThanOrEqual).Curry(end.Measure()));
            var fleft = fsplit.Left;
            var fright = fsplit.Right;
            var merged = Merge(rest, fright);

            return fleft.Merge(merged.PushLeft(end));
        }
    }
}
