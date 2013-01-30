/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
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
        /// When overridden in a derived class performs injection upon the given instance
        /// </summary>
        /// <param name="instance">The instance to perform injection upon</param>
        protected abstract void Inject(object instance);

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
                var registrars = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => (t.BaseType != null && t.BaseType.Equals(typeof(IOCRegistrar))) && t.GetConstructor(Type.EmptyTypes) != null)
                    .Select(t => (IOCRegistrar)Activator.CreateInstance(t));

                foreach (var registrar in registrars)
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
            return (T)this.GetInstance(typeof(T), args);
        }


        public object Get(Type serviceType, params IOCConstructorArgument[] args)
        {
            return this.GetInstance(serviceType, args);
        }


        public bool IsTypeBound<T>()
        {
            return this.IsTypeRegistered(typeof(T));
        }

        public bool IsTypeBound(Type serviceType)
        {
            return this.IsTypeRegistered(serviceType);
        }
    }
}
