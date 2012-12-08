using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Interface for a kernel whose implementation operates remotely across AppDomains (or simply by proxy)
    /// </summary>
    public interface IRemoteKernel
    {
        /// <summary>
        /// Gets an instance from the kernel
        /// </summary>
        /// <param name="type">The type of instance to get</param>
        /// <param name="args">The constructor arguments (name and value)</param>
        /// <returns>The instance</returns>
        object Get(Type type, params Tuple<string, object>[] args);

        /// <summary>
        /// Gets an instance from the kernel
        /// </summary>
        /// <typeparam name="T">The type of instance to get</typeparam>
        /// <param name="args">The constructor arguments (name and value)</param>
        /// <returns>The instance</returns>
        T Get<T>(params Tuple<string, object>[] args);
    }
}
