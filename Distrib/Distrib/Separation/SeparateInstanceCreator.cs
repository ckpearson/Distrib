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
using Distrib.IOC;
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

        private readonly Func<Type, bool> _iocHasType;
        private readonly Func<Type, object> _iocGetInstance;

        public SeparateInstanceCreator(Func<Type, bool> iocHasType, Func<Type, object> iocGetInstance,
            IRemoteDomainBridgeFactory bridgeFactory)
        {
            _iocHasType = iocHasType;
            _iocGetInstance = iocGetInstance;
            _bridgeFactory = bridgeFactory;
        }

        /// <summary>
        /// Creates an instance of the given type and auto-fills any IOC supplied arguments
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="args">The arguments that are required outside of IOC</param>
        /// <param name="funcGetInstance">The function that creates the actual instance using the final arguments</param>
        /// <returns>The instance</returns>
        private object _CreateInstance(Type type, IOCConstructorArgument[] args,
            Func<Type, object[], object> funcGetInstance)
        {
            if (type == null) throw new ArgumentNullException("type must be supplied");

            try
            {
                // Initialise args to empty if none provided
                if (args == null)
                {
                    args = new IOCConstructorArgument[] {};
                }

                // Make sure none of the args have duplicate names
                if (args.GroupBy(a => a.Name).Any(g => g.Count() > 1))
                {
                    throw new InvalidOperationException("Arguments must have unique names corresponding to the names expected by the constructor");
                }

                var constructors = type.GetConstructors().ToList();

                if (constructors.Count == 0 || constructors.Count > 1)
                {
                    throw new InvalidOperationException("Type must have one and only one constructor");
                }

                var constructor = constructors[0];

                var parameters = constructor.GetParameters();

                if (parameters.Length == 0)
                {
                    // No constructor parameters so just grab the instance
                    return funcGetInstance(type, null);
                }
                else
                {
                    if (parameters.Length == args.Length)
                    {
                        // Same number of parameters supplied as the constructor needs
                        // try to get the instance using the supplied arguments
                        return funcGetInstance(type, args.Select(kvp => kvp.Value).ToArray());
                    }
                    else if (parameters.Length < args.Length)
                    {
                        // The args supplied should have less items than the constructor requires
                        throw new InvalidOperationException("Too many arguments supplied");
                    }
                    else
                    {
                        // The parameters required by the constructor are more numerous than we have here
                        // there's a good chance some of the items are to be provided by the IOC system

                        var argsList = new object[parameters.Length];

                        // Iterate over the parameters
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var param = parameters[i];

                            // Check to see if this parameter is already supplied for by the args list

                            if (args.Count(a => a.Name == param.Name) == 1)
                            {
                                var arg = args.Single(a => a.Name == param.Name);

                                if (!param.ParameterType.IsAssignableFrom(arg.Value.GetType()))
                                {
                                    // The value provided can't be used for the parameter of the same name
                                    throw new InvalidOperationException(string.Format("argument '{0}' value isn't assignable from type '{1}' of " +
                                        "constructor parameter, '{2}' expected",
                                            arg.Name,
                                            arg.Value.GetType().FullName,
                                            param.ParameterType.FullName));
                                }
                                else
                                {
                                    // The value is provided for, chuck it into the args list
                                    argsList[i] = arg.Value;
                                    continue;
                                }
                            }
                            else
                            {
                                // The args list doesn't have a value to satisfy this, so it's quite likely to be something that would otherwise
                                // be provided for by the IOC container, check to see if it's explicitly marked as such first

                                bool isForIOC = true;   // Assume this is something the IOC system would provide

                                if (Attribute.IsDefined(param, typeof(IOCAttribute)))
                                {
                                    // The parameter on the constructor has been marked for this process
                                    // now to check whether it's stated as wanting to be supplied by IOC

                                    isForIOC = ((IOCAttribute)Attribute.GetCustomAttribute(param, typeof(IOCAttribute))).ForIOC;
                                }

                                if (!isForIOC)
                                {
                                    // The parameter isn't supposed to be supplied by IOC, and given we don't have anything to give it
                                    // from the args list, this is an exception

                                    throw new InvalidOperationException(string.Format("parameter '{0}' isn't to be supplied by IOC and no " +
                                        "value has been provided for it", param.Name));
                                }
                                else
                                {
                                    // It's either explicitly marked as needing to be supplied by IOC, or it's being assumed as such
                                    // time to check whether it can be

                                    if (!_iocHasType(param.ParameterType))
                                    {
                                        throw new InvalidOperationException(string.Format("IOC claims it cannot provide for type '{0}' on " +
                                            "parameter '{1}'", param.ParameterType.FullName, param.Name));
                                    }
                                    else
                                    {
                                        // IOC can provide the type, let it

                                        try 
                                        {
                                            argsList[i] = _iocGetInstance(param.ParameterType);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new ApplicationException(string.Format("Failed to get instance of type '{0}' from IOC for " +
                                                "parameter '{1}", param.ParameterType.FullName, param.Name), ex);
                                        }
                                    }
                                }
                            }
                        }

                        // Now we've gone over every parameter for the constructor time to return the instance

                        return funcGetInstance(type, argsList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create instance", ex);
            }
        }

        public object CreateInstanceSeparatedWithLoadedAssembly(Type type, string assemblyPath, IOCConstructorArgument[] args)
        {
            if (type == null) throw new ArgumentNullException("Type must be supplied");

            if (args == null)
            {
                args = new IOCConstructorArgument[] { };
            }

            try
            {
                return _CreateInstance(type, args,
                        (t, a) =>
                        {
                            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                            var bridge = _bridgeFactory.ForAppDomain(domain);
                            bridge.LoadAssembly(assemblyPath);
                            bridge.LoadAssembly(type.Assembly.Location);
                            domain.InitializeLifetimeService();
                            return bridge.CreateInstance(t.FullName, a);
                        });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create separated instance", ex);
            }
        }

        public object CreateInstanceWithSeparation(Type type, IOCConstructorArgument[] args)
        {
            if (type == null) throw new ArgumentNullException("Type must be supplied");

            if (args == null)
            {
                args = new IOCConstructorArgument[] { };
            }

            try
            {
                return _CreateInstance(type, args,
                        (t, a) =>
                        {
                            var typLoading = a[0] as Type;
                            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
                            var bridge = _bridgeFactory.ForAppDomain(domain);
                            bridge.LoadAssembly(type.Assembly.Location);
                            bridge.LoadAssembly(typLoading.Assembly.Location);
                            domain.InitializeLifetimeService();
                            return bridge.CreateInstance(t.FullName, a);
                        });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create separated instance", ex);
            }
        }



        public object CreateInstanceWithoutSeparation(Type type, IOCConstructorArgument[] args)
        {
            if (type == null) throw new ArgumentNullException("Type must be supplied");

            if (args == null)
            {
                args = new IOCConstructorArgument[] { };
            }

            try
            {
                return _CreateInstance(type, args,
                        (t, a) => Activator.CreateInstance(t, a));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create non-separated instance", ex);
            }
        }


        public T CreateInstanceWithSeparation<T>(IOCConstructorArgument[] args) where T : class
        {
            return (T)CreateInstanceWithSeparation(typeof(T), args);
        }


        public T CreateInstanceWithoutSeparation<T>(IOCConstructorArgument[] args) where T : class
        {
            return (T)CreateInstanceWithoutSeparation(typeof(T), args);
        }

        public T CreateInstanceSeparatedWithLoadedAssembly<T>(string assemblyPath, IOCConstructorArgument[] args) where T : class
        {
            return (T)CreateInstanceSeparatedWithLoadedAssembly(typeof(T), assemblyPath, args);
        }
    }
}
