using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Comms message for invoking a method
    /// </summary>
    [Serializable()]
    public sealed class MethodInvokeCommsMessage : CommsMessageBase, IMethodInvokeCommsMessage
    {
        private readonly string _methodName;
        private readonly object[] _invokeArgs;

        public MethodInvokeCommsMessage(string methodName, object[] invokeArgs)
            : base(CommsMessageType.MethodInvoke)
        {
            _methodName = methodName;
            _invokeArgs = invokeArgs;
        }

        public string MethodName
        {
            get { return _methodName; }
        }

        public object[] InvokeArgs
        {
            get { return _invokeArgs; }
        }
    }
}
