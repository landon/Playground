using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{ 
    public class Lazy<T>
    {
        Func<T> _creator;
        T _value = default(T);
        object _token = new object();

        public T Value
        {
            get
            {
                if (_value == null)
                {
                    lock (_token)
                    {
                        if (_value == null)
                            _value = _creator();
                    }
                }

                return _value;
            }
        }

        public Lazy(Func<T> creator)
        {
            _creator = creator;
        }

        public Lazy(Func<T> creator, bool threadSafe)
            : this(creator)
        {
        }

        public void Forget()
        {
            lock (_token)
                _value = default(T);
        }

        public static implicit operator T(Lazy<T> da)
        {
            return da.Value;
        }
    }
}
