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
    }
}
