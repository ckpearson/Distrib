using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class SetPropertyResultCommsMessage : CommsMessageBase, ISetPropertyResultCommsMessage
    {
        private readonly ISetPropertyCommsMessage _setMessage;

        public SetPropertyResultCommsMessage(ISetPropertyCommsMessage setMessage)
            : base(CommsMessageType.PropertySetResult)
        {
            _setMessage = setMessage;
        }

        public ISetPropertyCommsMessage SetMessage
        {
            get { return _setMessage; }
        }
    }
}
