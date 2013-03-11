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
    public interface IIOC
    {
        /// <summary>
        /// Gets an instance of the given service type
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <param name="args">The constructor arguments</param>
        /// <returns>The instance</returns>
        T Get<T>(params IOCConstructorArgument[] args);

        /// <summary>
        /// Gets an instance of the given service type
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="args">The constructor arguments</param>
        /// <returns>The instance</returns>
        object Get(Type serviceType, params IOCConstructorArgument[] args);

        /// <summary>
        /// Determines whether the given service type is bound
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns><c>True</c> if bound, <c>False</c> otherwise</returns>
        bool IsTypeBound<T>();

        /// <summary>
        /// Determines whether the given service type is bound
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns><c>True</c> if bound, <c>False</c> otherwise</returns>
        bool IsTypeBound(Type serviceType);

        /// <summary>
        /// Starts this bootstrapper off as the current bootstrapper
        /// </summary>
        void Start();

        /// <summary>
        /// Overrides an existing IOC binding
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method requires a binding to already exist, so, if you're overriding
        /// a Distrib binding, call this after the Start method has been called,
        /// otherwise be sure to call it after the binding has been made.
        /// </para>
        /// <para>
        /// This method allows continuous rebinding, so there is a danger that
        /// rebindings could themselves be rebound, keep this in mind!
        /// </para>
        /// </remarks>
        /// <typeparam name="TInterface">The service type</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="singleton">Whether the binding should function as a singleton</param>
        void Rebind<TInterface, TImplementation>(bool singleton)
            where TInterface : class
            where TImplementation : class, TInterface;

        /// <summary>
        /// Overrides an existing IOC binding with a constant implementation
        /// </summary>
        /// <typeparam name="TInterface">The service type</typeparam>
        /// <param name="instance">The constant implementation to bind to</param>
        void RebindToConstant<TInterface>(TInterface instance);
    }
}
