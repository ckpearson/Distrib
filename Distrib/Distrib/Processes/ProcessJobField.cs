using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    [Serializable()]
    internal class ProcessJobField : IProcessJobField
    {
        private readonly object _lock = new object();

        private readonly Type _fieldType;
        private readonly string _fieldName;
        private readonly FieldMode _fieldMode;

        private object _value;

        private IProcessJobFieldConfig _fieldConfig = ProcessJobFieldConfigFactory.CreateConfig();

        public ProcessJobField(Type fieldType, string fieldName, FieldMode fieldMode)
        {
            _fieldType = fieldType;
            _fieldName = fieldName;
            _fieldMode = fieldMode;
        }

        public Type Type
        {
            get { return _fieldType; }
        }

        public string Name
        {
            get { return _fieldName; }
        }

        public FieldMode Mode
        {
            get { return _fieldMode; }
        }

        public object Value
        {
            get
            {
                lock(_lock)
                {
                    return _value;
                }
            }

            set
            {
               lock(_lock)
               {
                   _value = value;
               }
            }
        }

        public IProcessJobFieldConfig Config
        {
            get { return _fieldConfig; }
            protected set
            {
                lock (_lock)
                {
                    _fieldConfig = value;
                }
            }
        }
    }

    [Serializable()]
    internal sealed class ProcessJobField<T> : ProcessJobField, IProcessJobField<T>
    {
        public ProcessJobField(string fieldName, FieldMode mode)
            : base(typeof(T), fieldName, mode)
        {
            base.Config = ProcessJobFieldConfigFactory.CreateConfig<T>();
        }

        public new T Value
        {
            get
            {
                return (T)base.Value;
            }
            set
            {
                base.Value = value;
            }
        }

        public new IProcessJobFieldConfig<T> Config
        {
            get { return (IProcessJobFieldConfig<T>)base.Config; }
        }
    }
}
