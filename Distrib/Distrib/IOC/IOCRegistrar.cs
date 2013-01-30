using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Base class for an IOC registrar used for registering IOC bindings
    /// </summary>
    public abstract class IOCRegistrar
    {
        public abstract void PerformBindings();

        /// <summary>
        /// Binds the given service type to an implementation type
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="concreteType">The implementation type</param>
        protected void Bind(Type serviceType, Type concreteType)
        {
            Ex.ArgNull(() => serviceType);
            Ex.ArgNull(() => concreteType);

            try
            {
                if (DistribBootstrapper.CurrentBoostrapper == null)
                {
                    throw new InvalidOperationException("No bootstrapper has been started");
                }

                DistribBootstrapper.CurrentBoostrapper.DoBind(serviceType, concreteType, false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform binding", ex);
            }
        }

        /// <summary>
        /// Binds the given service type to an implementation type
        /// </summary>
        /// <typeparam name="TFrom">The service type</typeparam>
        /// <typeparam name="TTO">The implementation type</typeparam>
        protected void Bind<TFrom, TTO>()
            where TFrom : class
            where TTO : class
        {
            Bind(typeof(TFrom), typeof(TTO));
        }

        /// <summary>
        /// Binds the given service type to an implementation type in a singleton scope
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="concreteType">The implementation type</param>
        protected void BindSingleton(Type serviceType, Type concreteType)
        {
            Ex.ArgNull(() => serviceType);
            Ex.ArgNull(() => concreteType);

            try
            {
                if (DistribBootstrapper.CurrentBoostrapper == null)
                {
                    throw new InvalidOperationException("No bootstrapper has been started");
                }

                DistribBootstrapper.CurrentBoostrapper.DoBind(serviceType, concreteType, true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform binding for singleton", ex);
            }
        }

        /// <summary>
        /// Binds the given service type to an implementation type in a singleton scope
        /// </summary>
        /// <typeparam name="TFrom">The service type</typeparam>
        /// <typeparam name="TTO">The implementation type</typeparam>
        protected void BindSingleton<TFrom, TTO>()
            where TFrom : class
            where TTO : class
        {
            BindSingleton(typeof(TFrom), typeof(TTO));
        }
    }
}
