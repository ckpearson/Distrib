using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class GetPropertyResultCommsMessage : CommsMessageBase, IGetPropertyResultCommsMessage
    {
        private readonly IGetPropertyCommsMessage _propertyGetMessage;
        private readonly object _value;

        public GetPropertyResultCommsMessage(IGetPropertyCommsMessage propertyGetMessage, object value)
            : base(CommsMessageType.PropertyGetResult)
        {
            _propertyGetMessage = propertyGetMessage;
            _value = value;
        }

        public IGetPropertyCommsMessage PropertyGetMessage
        {
            get { return _propertyGetMessage; }
        }

        public object Value
        {
            get { return _value; }
        }
    }
}
