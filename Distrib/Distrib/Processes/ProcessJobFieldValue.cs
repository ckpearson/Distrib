using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    [Serializable()]
    internal class ProcessJobFieldValue : IProcessJobValueField
    {
        private readonly IProcessJobDefinitionField _definition;
        private object _value;
        private readonly object _lock = new object();

        public ProcessJobFieldValue(IProcessJobDefinitionField definition)
        {
            _definition = definition;
        }

        public IProcessJobDefinitionField Definition
        {
            get { return _definition; }
        }

        public object Value
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
                }
            }
        }
    }

    [Serializable()]
    internal sealed class ProcessJobFieldValue<T> : ProcessJobFieldValue, IProcessJobValueField<T>
    {
        public ProcessJobFieldValue(IProcessJobDefinitionField<T> definition)
            : base(definition)
        {

        }

        public new IProcessJobDefinitionField<T> Definition
        {
            get { return (IProcessJobDefinitionField<T>)base.Definition; }
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
    }
}
