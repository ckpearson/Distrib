using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class GetPropertyCommsMessage : CommsMessageBase, IGetPropertyCommsMessage
    {
        private readonly string _propertyName;

        public GetPropertyCommsMessage(string propertyName)
            : base(CommsMessageType.PropertyGet)
        {
            _propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }
    }
}
