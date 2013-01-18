using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class DeferredValueProvider<TVal, TType> : CrossAppDomainObject, IDeferredValueProvider<TVal>
        where TType : IDeferredValueSource<TVal>
    {
        private IDeferredValueSource<TVal> _inst;
        private object _lock = new object();
        private readonly string _typeName;

        private TrackWritten<TVal> _value = new TrackWritten<TVal>(default(TVal));

        private readonly DeferredValueCacheMode _cacheMode;

        public DeferredValueProvider(DeferredValueCacheMode cacheMode)
        {
            _cacheMode = cacheMode;
            _typeName = typeof(TType).FullName;

            var typ = typeof(TType);
            if (!typ.IsClass) throw new InvalidOperationException("TType must be a class");
            if (typ.GetConstructor(Type.EmptyTypes) == null) throw new InvalidOperationException("TType must have a parameterless constructor");
            if (typ.BaseType == null || typ.BaseType.Equals(typeof(CrossAppDomainObject)) == false)
                throw new InvalidOperationException("TType must derive from CrossAppDomainObject");
        }

        private void _initInst()
        {
            lock (_lock)
            {
                if (_inst == null)
                {
                    _inst = (IDeferredValueSource<TVal>)Activator.CreateInstance<TType>();
                }
            }
        }

        private object _getValue()
        {
            lock (_lock)
            {
                _initInst();
                Func<TVal> read = new Func<TVal>(() => _inst.ProvideValue());
                switch (_cacheMode)
                {
                    case DeferredValueCacheMode.ReadOnceAndCache:
                        if (_value.Written)
                        {
                            return _value.Value;
                        }
                        else
                        {
                            _value.Value = read();
                            return _value.Value;
                        }

                    default:

                    case DeferredValueCacheMode.ReadAlways:

                        return read();
                }
            }
        }

        public TVal RetrieveValue()
        {
            lock (_lock)
            {
                return (TVal)_getValue();
            }
        }

        object IDeferredValueProvider.RetrieveValue()
        {
            lock (_lock)
            {
                return _getValue();
            }
        }
    }
}
