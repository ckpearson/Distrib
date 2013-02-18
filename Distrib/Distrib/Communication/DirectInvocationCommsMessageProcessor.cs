using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                if (target.GetType().GetProperty(msg.PropertyName) == null)
                {
                    throw new ApplicationException("Property not present on target type");
                }

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
                if (target.GetType().GetProperty(msg.PropertyName) == null)
                {
                    throw new ApplicationException("Property not present on target type");
                }

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

        private Dictionary<string, Delegate> _dictMethodInvokeCache =
            new Dictionary<string, Delegate>();

        private ICommsMessage _handleMethodInvoke(object target, IMethodInvokeCommsMessage msg)
        {
            if (target == null) throw Ex.ArgNull(() => target);
            if (msg == null) throw Ex.ArgNull(() => msg);
            if (msg.Type != CommsMessageType.MethodInvoke) throw Ex.Arg(() => msg,
                "The message should be a method invoke comms message");

            try
            {
                if (_dictMethodInvokeCache.ContainsKey(msg.MethodName))
                {
                    return new MethodInvokeResultCommsMessage(msg,
                        _dictMethodInvokeCache[msg.MethodName].DynamicInvoke(msg.InvokeArgs));
                }
                else
                {
                    if (target.GetType().GetMethod(msg.MethodName) == null)
                    {
                        throw new InvalidOperationException("Method not present on target type");
                    }

                    var call = Expression.Call(Expression.Constant(target),
                        msg.MethodName,
                        null,
                        target.GetType().GetMethod(msg.MethodName)
                            .GetParameters()
                            .Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray());

                    var lamb = Expression.Lambda(call, call.Arguments.Cast<ParameterExpression>().ToArray()).Compile();
                    _dictMethodInvokeCache[msg.MethodName] = lamb;
                    return new MethodInvokeResultCommsMessage(msg, lamb.DynamicInvoke(msg.InvokeArgs));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to process method invoke message", ex);
            }
        }
    }
}
