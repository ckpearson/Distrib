using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Comms message processor that just directly invokes against the target
    /// </summary>
    public sealed class DirectInvocationCommsMessageProcessor : IIncomingCommsMessageProcessor
    {
        public ICommsMessage ProcessMessage(object target, ICommsMessage msg)
        {
            switch (msg.Type)
            {
                case CommsMessageType.MethodInvoke:
                    return _handleMethodInvoke(target, (IMethodInvokeCommsMessage)msg);

                case CommsMessageType.PropertyGet:
                    return _handlePropertyGet(target, (IGetPropertyCommsMessage)msg);

                case CommsMessageType.PropertySet:
                    return _handlePropertySet(target, (ISetPropertyCommsMessage)msg);

                case CommsMessageType.MethodInvokeResult:
                case CommsMessageType.PropertyGetResult:
                case CommsMessageType.PropertySetResult:

                    throw Ex.Arg(() => msg,
                        "A result comms message isn't valid for incoming communication");

                case CommsMessageType.Exception:

                    throw Ex.Arg(() => msg,
                        "An exception comms message isn't valid this far on the incoming stack");

                case CommsMessageType.Unknown:
                default:
                    throw Ex.Arg(() => msg,
                        string.Format("A comms message of type '{0}' isn't supported for incoming",
                            msg.Type.ToString()));
            }
        }

        private ICommsMessage _handlePropertySet(object target, ISetPropertyCommsMessage msg)
        {
            if (target == null) throw Ex.ArgNull(() => target);
            if (msg == null) throw Ex.ArgNull(() => msg);
            if (msg.Type != CommsMessageType.PropertySet) throw Ex.Arg(() => msg,
                "The message should be a property set comms message");

            try
            {
                target.GetType()
                    .GetProperty(msg.PropertyName)
                    .SetValue(target, msg.Value);

                return new SetPropertyResultCommsMessage(msg);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process property set message", ex);
            }
        }

        private ICommsMessage _handlePropertyGet(object target, IGetPropertyCommsMessage msg)
        {
            if (target == null) throw Ex.ArgNull(() => target);
            if (msg == null) throw Ex.ArgNull(() => msg);
            if (msg.Type != CommsMessageType.PropertyGet) throw Ex.Arg(() => msg,
                "The message should be a property get comms message");

            try
            {
                return new GetPropertyResultCommsMessage(msg,
                    target.GetType()
                        .GetProperty(msg.PropertyName)
                        .GetValue(target));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process property get message", ex);
            }
        }

        private ICommsMessage _handleMethodInvoke(object target, IMethodInvokeCommsMessage msg)
        {
            if (target == null) throw Ex.ArgNull(() => target);
            if (msg == null) throw Ex.ArgNull(() => msg);
            if (msg.Type != CommsMessageType.MethodInvoke) throw Ex.Arg(() => msg,
                "The message should be a method invoke comms message");

            try
            {
                return new MethodInvokeResultCommsMessage(msg,
                    target.GetType()
                        .GetMethod(msg.MethodName)
                        .Invoke(target, msg.InvokeArgs));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process method invoke message", ex);
            }
        }
    }
}
