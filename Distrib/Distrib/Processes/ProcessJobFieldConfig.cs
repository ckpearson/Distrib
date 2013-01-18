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

        private LockValue<TrackWritten<object>> _defaultValue = new LockValue<TrackWritten<object>>
            (new TrackWritten<object>(null));

        public object DefaultValue
        {
            get
            {
                return _defaultValue.Value.Value;
            }
            set
            {
                _defaultValue.Value.Value = value;
            }
        }

        public bool HasDefaultValue
        {
            get { return _defaultValue.Value.Written; }
        }

        public void Adopt(IProcessJobFieldConfig config)
        {
            this.DefaultValue = config.DefaultValue;
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldConfig<T> : ProcessJobFieldConfig, IProcessJobFieldConfig<T>
    {
        public new T DefaultValue
        {
            get
            {
                return (T)base.DefaultValue;
            }
            set
            {
                base.DefaultValue = value;
            }
        }
    }
}
