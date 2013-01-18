using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    /// <summary>
    /// Creates instances of a given type separated into its own AppDomain
    /// </summary>
    public interface ISeparateInstanceCreator
    {
        /// <summary>
        /// Create an instance of the given type, separated by its own AppDomain
        /// </summary>
        /// <param name="type">The type of the instance to create</param>
        /// <param name="args">The constructor arguments required, those bound with IOC will be provided automatically.</param>
        /// <returns>The instance</returns>
        object CreateInstanceWithSeparation(Type type, KeyValuePair<string, object>[] args);

        /// <summary>
        /// Creates an instance of the given type, no separation is used
        /// </summary>
        /// <param name="type">The type of the instance to create</param>
        /// <param name="args">The constructor arguments required, those bound with IOC will be provided automatically.</param>
        /// <returns>The instance</returns>
        object CreateInstanceWithoutSeparation(Type type, KeyValuePair<string, object>[] args);

        object CreateInstanceSeparatedWithLoadedAssembly(Type type, string assemblyPath, KeyValuePair<string, object>[] args);
    }
}
