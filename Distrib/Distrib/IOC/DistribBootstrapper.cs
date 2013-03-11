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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Base class for an IOC bootstrapper for utilisation by Distrib
    /// </summary>
    public abstract class DistribBootstrapper : CrossAppDomainObject, IIOC
    {
        private static DistribBootstrapper _currentBootstrapper = null;
        private static object _lock = new object();

        private readonly bool _attemptArgumentMatching = false;

        private IReadOnlyList<IOCRegistrar> _registrars;


        public DistribBootstrapper()
            : this(true)
        {

        }

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="attemptArgumentMatching">Whether to attempt automatic argument matching</param>
        public DistribBootstrapper(bool attemptArgumentMatching)
        {
            _attemptArgumentMatching = attemptArgumentMatching;
        }

        /// <summary>
        /// Gets or sets the current active bootstrapper being used by Distrib
        /// </summary>
        internal static DistribBootstrapper CurrentBoostrapper
        {
            get
            {
                lock (_lock)
                {
                    return _currentBootstrapper;
                }
            }
            set
            {
                lock (_lock)
                {
                    _currentBootstrapper = value;
                }
            }
        }

        /// <summary>
        /// When overriden in a derived class performs any initialisation prior to IOC bootstrapping 
        /// being performed
        /// </summary>
        protected virtual void Initialise() { }

        /// <summary>
        /// When overridden in a derived class gets an instance of the given service type
        /// </summary>
        /// <param name="serviceType">The service type to return an instance of</param>
        /// <param name="args">The constructor arguments to use</param>
        /// <returns>The instance</returns>
        protected abstract object GetInstance(Type serviceType, params IOCConstructorArgument[] args);

        /// <summary>
        /// When overridden in a derived class gets all the instances of the given service type
        /// </summary>
        /// <param name="serviceType">The servive type to return all the instances of</param>
        /// <returns>The instances of the given service type</returns>
        protected abstract IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// When overridden in a derived class binds the given service type to an implementation type
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="concreteType">The implementation type</param>
        /// <param name="singleton">Whether to bind the implementation as a singleton</param>
        protected abstract void Bind(Type serviceType, Type concreteType, bool singleton = false);

        /// <summary>
        /// When overridden in a derived class binds the given service type to a constant
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="instance">The constant to bind the service type to</param>
        protected abstract void BindToConstant(Type serviceType, object instance);

        /// <summary>
        /// When overridden in a derived class determines whether the given service type has been registered
        /// with the container
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns><c>True</c> if the type has been registered, <c>False</c> otherwise</returns>
        protected abstract bool IsTypeRegistered(Type serviceType);

        /// <summary>
        /// When overridden in a derived class, rebinds a given service type to a new instance type
        /// in a chosen scope
        /// </summary>
        /// <param name="serviceType">The service type to rebind</param>
        /// <param name="concreteType">The implementation type to bind to</param>
        /// <param name="singleton">Whether the binding should be in the singleton scope</param>
        protected abstract void Rebind(Type serviceType, Type concreteType, bool singleton);

        /// <summary>
        /// When overridden in a derived class, rebinds a given service type to a given instance
        /// </summary>
        /// <param name="serviceType">The service type to rebind</param>
        /// <param name="instance">The instance to bind to</param>
        protected abstract void RebindToConstant(Type serviceType, object instance);


        public void Start()
        {
            try
            {
                if (DistribBootstrapper.CurrentBoostrapper != null)
                {
                    throw new InvalidOperationException("A boostrapper has already been started");
                }
                else
                {
                    DistribBootstrapper.CurrentBoostrapper = this;
                }

                this.Initialise();

                // Ensure that Distrib can access the IOC goodness
                this.BindToConstant(typeof(IIOC), this);

                // Now go out and ask all the IOC registrar classes to chime in
                _registrars = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => (t.BaseType != null && t.BaseType.Equals(typeof(IOCRegistrar))) && t.GetConstructor(Type.EmptyTypes) != null)
                    .Select(t => (IOCRegistrar)Activator.CreateInstance(t)).ToList().AsReadOnly();

                foreach (var registrar in _registrars)
                {
                    registrar.PerformBindings();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to start bootstrapping for Distrib", ex);
            }
        }

        internal void DoBind(Type serviceType, Type concreteType, bool singleton = false)
        {
            this.Bind(serviceType, concreteType, singleton);
        }

        public T Get<T>(params IOCConstructorArgument[] args)
        {
            return (T)this.GetInstance(typeof(T), _ArgMatchIfAble(typeof(T), args));
        }


        public object Get(Type serviceType, params IOCConstructorArgument[] args)
        {
            return this.GetInstance(serviceType, _ArgMatchIfAble(serviceType, args));
        }

        /// <summary>
        /// Attempt to perform some naive argument matching if able to do so
        /// </summary>
        /// <param name="serviceType">The service type to attempt argument matching for</param>
        /// <param name="args">The arguments already provided with the request</param>
        /// <returns>The resulting arguments post-matching (if performed)</returns>
        private IOCConstructorArgument[] _ArgMatchIfAble(Type serviceType, IOCConstructorArgument[] args)
        {
            if (serviceType == null) throw Ex.ArgNull(() => serviceType);

            try
            {
                if (!_attemptArgumentMatching)
                {
                    return args;
                }

                if (args == null)
                {
                    return args;
                }

                var bindingEntry = _getBindingEntry(serviceType);

                if (bindingEntry == null)
                {
                    throw new InvalidOperationException(string.Format("A single binding entry for service type '{0}' couldn't be found",
                    serviceType.FullName));
                }

                var constructors = bindingEntry.Implementation.GetConstructors();

                if (constructors.Length > 1)
                {
                    // Due to complexities of how the IOC frameworks may determine the correct constructor
                    // the argument matching can only be (relatively) sure of success with the one ctor
                    return args;
                }

                var ctorParameters = constructors[0].GetParameters();

                if (ctorParameters == null || ctorParameters.Length == 0)
                {
                    return args;
                }

                foreach (var param in ctorParameters)
                {
                    bool tryDoMatch = true;

                    if (Attribute.IsDefined(param, typeof(IOCAttribute)))
                    {
                        // The IOC attribute is on the parameter so that'll tell us if
                        // it's expecting the parameter to be injected by IOC

                        tryDoMatch = !param.GetCustomAttribute<IOCAttribute>().ForIOC;
                    }

                    if (param.ParameterType.Equals(typeof(IIOC)))
                    {
                        // No point in trying to match an IIOC argument
                        tryDoMatch = false;
                    }

                    if (!tryDoMatch)
                    {
                        // So far because there's no match requirement this is an IOC injected arg
                        // so it's not needed in the arguments list
                        continue;
                    }

                    // Look for the argument with the matching name
                    var foundArg = args.SingleOrDefault(arg => arg.Name == param.Name);

                    if (foundArg != null)
                    {
                        // There's an argument supplied whose name matches the parameter name
                        // Just leave this one be
                        continue;
                    }
                    else
                    {
                        // Look for the argument with the matching type
                        foundArg = args.SingleOrDefault(arg => arg.Value.GetType().Equals(param.ParameterType));

                        if (foundArg != null)
                        {
                            var indx = Array.IndexOf(args, foundArg);
                            args[indx] = new IOCConstructorArgument(param.Name, foundArg.Value);
                            continue;
                        }
                        else
                        {
                            // Look for the argument with an assignable type in the event the parameter
                            // is looking for an interface or a base-type
                            foundArg = args.SingleOrDefault(arg => param.ParameterType.IsAssignableFrom(arg.Value.GetType()));

                            if (foundArg != null)
                            {
                                var indx = Array.IndexOf(args, foundArg);
                                args[indx] = new IOCConstructorArgument(param.Name, foundArg.Value);
                                continue;
                            }
                        }
                    }
                }

                return args;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Bootstrapper failed to perform argument matching", ex);
            }
        }


        public bool IsTypeBound<T>()
        {
            return this.IsTypeRegistered(typeof(T));
        }

        public bool IsTypeBound(Type serviceType)
        {
            if (serviceType == null) throw Ex.ArgNull(() => serviceType);

            return this.IsTypeRegistered(serviceType);
        }


        public void Rebind<TInterface, TImplementation>(bool singleton)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            try
            {
                _updateBindingEntry<TInterface>((entry) =>
                    {
                        entry.IsSingleton = singleton;
                        entry.Implementation = typeof(TImplementation);
                    });

                this.Rebind(typeof(TInterface), typeof(TImplementation), singleton);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to rebind service type", ex);
            }
        }

        private IOCRegistrar.BindingEntry _getBindingEntry(Type serviceType)
        {
            try
            {
                return _registrars.SelectMany(reg => reg.BindingEntries)
                        .SingleOrDefault(be => be.Service == serviceType);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get binding entry", ex);
            }
        }

        private void _updateBindingEntry<TService>(Action<IOCRegistrar.BindingEntry> updater)
        {
            try
            {
                var entry = _getBindingEntry(typeof(TService));

                if (entry == null)
                {
                    throw new InvalidOperationException("Failed to find binding entry");
                }

                updater(entry);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update binding entry", ex);
            }
        }


        public void RebindToConstant<TInterface>(TInterface instance)
        {
            if (instance == null) throw Ex.ArgNull(() => instance);

            _updateBindingEntry<TInterface>((entry) =>
                {
                    entry.IsSingleton = false;
                    entry.Implementation = instance.GetType();
                });

            this.RebindToConstant(typeof(TInterface), instance);
        }

    }
}
