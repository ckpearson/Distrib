using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents an outgoing comms link for direct invocation used when objects are reachable directly
    /// </summary>
    public class DirectInvokeOutgoingCommsLink : IOutgoingCommsLink
    {
        private readonly DirectInvokeCommsBridge _bridge;

        public DirectInvokeOutgoingCommsLink(DirectInvokeCommsBridge bridge)
        {
            _bridge = bridge;
        }

        public object InvokeMethod(object[] args, string methodName = "")
        {
            if (string.IsNullOrEmpty(methodName)) throw Ex.ArgNull(() => methodName);

            try
            {
                var result = ((IMethodInvokeResultCommsMessage)_bridge.SendMessage(
                    new MethodInvokeCommsMessage(methodName, args)));

                return result.ReturnValue;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to invoke method over outgoing comms link", ex);
            }
        }

        public object GetProperty(string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName)) throw Ex.ArgNull(() => propertyName);

            try
            {
                var result = ((IGetPropertyResultCommsMessage)_bridge.SendMessage(
                    new GetPropertyCommsMessage(propertyName)));

                return result.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get property over outgoing comms link", ex);
            }
        }

        public void SetProperty(object value, string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName)) throw Ex.ArgNull(() => propertyName);

            try
            {
                var result = ((ISetPropertyResultCommsMessage)_bridge.SendMessage(
                    new SetPropertyCommsMessage(propertyName, value)));

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to set property over outgoing comms link", ex);
            }
        }

        public CommsDirection PrimaryDirection
        {
            get { return CommsDirection.Outgoing; }
        }
    }

    public class DirectInvokeOutgoingCommsLink<T> : DirectInvokeOutgoingCommsLink, IOutgoingCommsLink<T> where T : class
    {
        public DirectInvokeOutgoingCommsLink(DirectInvokeCommsBridge bridge)
            : base(bridge) { }
    }

}
