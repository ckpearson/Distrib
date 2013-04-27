/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                    var typ = target.GetType();
                    MethodInfo chosenMethod = typ.GetMethod(msg.MethodName);

                    if (chosenMethod == null)
                    {
                        // The method couldn't be found, it could be because it's implemented explicitly
                        // try to find an explicit hit via the interfaces

                        var interfaces = typ.GetInterfaces();

                        if (interfaces != null && interfaces.Length > 0)
                        {
                            var methNames =
                                interfaces.Where(i => i.GetMethod(msg.MethodName) != null)
                                .Select(i => new
                                {
                                    explicitName = i.Name + "." + msg.MethodName,
                                    iface = i,
                                    meth = i.GetMethod(msg.MethodName),
                                }).ToArray();

                            if (methNames == null || methNames.Length == 0)
                            {
                                throw new InvalidOperationException("Method not present on target type " +
                            "and none of the implemented interfaces held a method for explicit resolution");
                            }
                            else
                            {
                                // There's a slight possibility of method names matching
                                // for more than one explicit implementation
                                // it's less likely they match on argument signatures, so
                                // identify the method that has the right signature

                                List<MethodInfo> lstFoundMethods = new List<MethodInfo>();

                                foreach (var mn in methNames)
                                {
                                    var paras = mn.meth.GetParameters();
                                    
                                    if (paras == null || paras.Length == 0 &&
                                        (msg.InvokeArgs == null || msg.InvokeArgs.Length == 0))
                                    {
                                        // No arguments supplied or expected, this is a candidate
                                        lstFoundMethods.Add(mn.meth);
                                    }
                                    else
                                    {
                                        // There are some arguments
                                        if (paras.Length == msg.InvokeArgs.Length)
                                        {
                                            // Only care if the same number of arguments count
                                            // Check each argument has the same type

                                            bool isMatch = true;

                                            for (int i = 0; i < paras.Length; i++)
                                            {
                                                if (!paras[i].ParameterType.IsAssignableFrom(msg.InvokeArgs[i].GetType()))
                                                {
                                                    isMatch = false;
                                                    break;
                                                }
                                            }

                                            if (isMatch)
                                            {
                                                lstFoundMethods.Add(mn.meth);
                                            }
                                        }
                                    }
                                }

                                if (lstFoundMethods.Count == 0)
                                {
                                    throw new InvalidOperationException("Method not present on target type " +
                            "and no interfaces had suitable methods for selection");
                                }
                                else if (lstFoundMethods.Count > 1)
                                {
                                    throw new InvalidOperationException("Method not present on target type " +
                            "and multiple interfaces had suitable methods for selection, choice was ambiguous");
                                }
                                else
                                {
                                    // We have exactly one suitable method at the end of it all
                                    chosenMethod = lstFoundMethods[0];
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Method not present on target type " + 
                            "and no interfaces were available for explicit implementation resolution");
                        }
                    }

                    var call = Expression.Call(Expression.Constant(target),
                        chosenMethod,
                        chosenMethod
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
