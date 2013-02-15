using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Comms message for when an exception occurred during message processing
    /// </summary>
    [Serializable()]
    public sealed class ExceptionCommsMessage : CommsMessageBase, IExceptionCommsMessage
    {
        private readonly ICommsMessage _causingMessage;
        private readonly Exception _exception;

        public ExceptionCommsMessage(ICommsMessage causingMessage, Exception exception)
            : base(CommsMessageType.Exception)
        {
            _causingMessage = causingMessage;
            _exception = exception;
        }

        public ICommsMessage CausingMessage
        {
            get { return _causingMessage; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
