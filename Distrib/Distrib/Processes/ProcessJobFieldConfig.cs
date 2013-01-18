using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    [Serializable()]
    internal class ProcessJobFieldConfig : IProcessJobFieldConfig, IProcessJobFieldConfig_Internal
    {
        protected readonly object _lock = new object();

        private LockValue<object> _defaultValue = new LockValue<object>(null);

        public object DefaultValue
        {
            get
            {
                return _defaultValue.Value;
            }
            set
            {
                _defaultValue.Value = value;
            }
        }

        public bool HasDefaultValue
        {
            get { return _defaultValue.Value != null; }
        }

        public void Adopt(IProcessJobFieldConfig config)
        {
            this.DefaultValue = config.DefaultValue;
            this.DeferredValueProvider = config.DeferredValueProvider;
        }

        private LockValue<IDeferredValueProvider> _deferredValueProvider = new LockValue<IDeferredValueProvider>(null);
        public IDeferredValueProvider DeferredValueProvider
        {
            get { return _deferredValueProvider.Value; }
            set
            {
                _deferredValueProvider.Value = value;
            }
        }


        public bool HasDeferredValueProvider
        {
            get { return _deferredValueProvider.Value != null; }
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldConfig<T> : ProcessJobFieldConfig, IProcessJobFieldConfig<T>
    {
        public new IDeferredValueProvider<T> DeferredValueProvider
        {
            get { return (IDeferredValueProvider<T>)base.DeferredValueProvider; }
            set { base.DeferredValueProvider = value; }
        }

        public new T DefaultValue
        {
            get
            {
                if (!base.HasDefaultValue)
                {
                    return default(T);
                }
                else
                {
                    return (T)base.DefaultValue;
                }
            }
            set
            {
                base.DefaultValue = value;
            }
        }
    }
}
