using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class SetPropertyCommsMessage : CommsMessageBase, ISetPropertyCommsMessage
    {
        private readonly string _propertyName;
        private readonly object _value;

        public SetPropertyCommsMessage(string propertyName, object value)
            : base(CommsMessageType.PropertySet)
        {
            _propertyName = propertyName;
            _value = value;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
