using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Comms message for method invocation result
    /// </summary>
    [Serializable()]
    public sealed class MethodInvokeResultCommsMessage : CommsMessageBase, IMethodInvokeResultCommsMessage
    {
        private readonly IMethodInvokeCommsMessage _invokeMessage;
        private readonly object _returnValue;

        public MethodInvokeResultCommsMessage(IMethodInvokeCommsMessage invokeMessage,
            object returnValue)
            : base(CommsMessageType.MethodInvokeResult)
        {
            _invokeMessage = invokeMessage;
            _returnValue = returnValue;
        }

        public IMethodInvokeCommsMessage InvokeMessage
        {
            get { return _invokeMessage; }
        }

        public object ReturnValue
        {
            get { return _returnValue; }
        }
    }
}
