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
