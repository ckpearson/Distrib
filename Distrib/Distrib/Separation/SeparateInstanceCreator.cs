using Distrib.IOC;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public sealed class SeparateInstanceCreator : ISeparateInstanceCreator
    {
        private readonly IRemoteDomainBridgeFactory _bridgeFactory;
        private readonly IKernel _kernel;

        public SeparateInstanceCreator(IKernel kernel,
            IRemoteDomainBridgeFactory bridgeFactory)
        {
            _kernel = kernel;
            _bridgeFactory = bridgeFactory;
        }

        public object CreateInstance(Type type, object[] args)
        {
            var ad = AppDomain.CreateDomain(Guid.NewGuid().ToString());
            IRemoteDomainBridge bridge = _bridgeFactory.ForAppDomain(ad);

            bridge.LoadAssembly(type.Assembly.Location);

            var consts = type.GetConstructors().ToList();

            if (consts.Count == 0 || consts.Count > 1)
            {
                throw new NotImplementedException("Instancing with objects with more than 1 or no constructors");
            }
            else
            {
                var c = consts[0];

                var par = c.GetParameters();

                if (par.Length == 0)
                {
                    return bridge.CreateInstance(type.FullName, null);
                }
                else
                {
                    if (par.Length == args.Length)
                    {
                        return bridge.CreateInstance(type.FullName, args);
                    }
                    else
                    {
                        var argsList = new object[par.Length];

                        // If the constructor takes less than what we have here there's something silly going on
                        if (par.Length < args.Length)
                        {
                            throw new InvalidOperationException("Too many arguments supplied");
                        }
                        else
                        {
                            for (int i = 0; i < par.Length; i++)
                            {
                                var param = par[i];

                                CustomAttributeData iocAttr = null;

                                if (param.CustomAttributes != null && param.CustomAttributes.Count() > 0)
                                {
                                    iocAttr = param.CustomAttributes
                                     .DefaultIfEmpty(null)
                                     .SingleOrDefault(a => a.AttributeType.Equals(typeof(IOCAttribute)));
                                }
                                
                                IOCAttribute iocAttr_Act = null;

                                if (iocAttr != null)
                                {
                                    iocAttr_Act = Attribute.GetCustomAttribute(param, typeof(IOCAttribute))
                                        as IOCAttribute;
                                }

                                if (iocAttr == null || iocAttr_Act.ForIOC == false)
                                {
                                    // the parameter hasn't got the IOC attribute on it, check there's
                                    // one arg of the same type waiting in the args array
                                    if (args.Count(arg => param.ParameterType.IsAssignableFrom(arg.GetType())) > 0)
                                    {
                                        // There is one, put it in at the same position
                                        argsList[i] = args[i];
                                        continue;
                                    }
                                    else
                                    {
                                        // it's not marked as an IOC attribute, but let's see if it can be provided by IOC
                                        var bindings = _kernel.GetBindings(param.ParameterType);

                                        if (bindings == null || bindings.Count() == 0)
                                        {
                                            throw new InvalidOperationException("Non IOC marked parameter needs only 1 matching argument of type");
                                        }
                                        else if (bindings != null && bindings.Count() > 0)
                                        {
                                            // There isn't a param for it, and it is resolvable via IOC, so just use it
                                            argsList[i] = _kernel.Get(param.ParameterType);
                                        }
                                    }
                                }
                                else if (iocAttr != null && iocAttr_Act.ForIOC == true)
                                {
                                    // Is marked as IOC provided, check it can be
                                    var bindings = _kernel.GetBindings(param.ParameterType);

                                    if (bindings == null || bindings.Count() == 0)
                                    {
                                        throw new InvalidOperationException("IOC marked attribute cannot be provided by IOC");
                                    }
                                    else
                                    {
                                        argsList[i] = _kernel.Get(param.ParameterType);
                                    }
                                }
                            }
                        }

                        return bridge.CreateInstance(type.FullName, argsList);
                    }
                }
            }
        }
    }
}
