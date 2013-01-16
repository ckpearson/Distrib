using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    [Serializable()]
    public sealed class TrackWritten<T>
    {
        private T _value;
        private object _lock = new object();
        private bool _written = false;

        public TrackWritten(T initialValue = default(T))
        {
            _value = initialValue;
            _written = false;
        }

        public T Value
        {
            get
            {
                lock (_lock)
                {
                    return _value;
                }
            }

            set
            {
                lock (_lock)
                {
                    _value = value;
                    if (!_written)
                        _written = true;
                }
            }
        }

        public bool Written
        {
            get
            {
                lock (_lock)
                {
                    return _written;
                }
            }
        }
    }
}
