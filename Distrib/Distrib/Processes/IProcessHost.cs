using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents a process host
    /// </summary>
    public interface IProcessHost
    {
        /// <summary>
        /// Initialises the process host
        /// </summary>
        void Initialise();

        /// <summary>
        /// Unitialises the process host
        /// </summary>
        void Unitialise();

        /// <summary>
        /// Gets whether the process host has been initialised
        /// </summary>
        bool IsInitialised { get; }

        /// <summary>
        /// Create and process a job using the given input values
        /// </summary>
        /// <param name="inputValues">The input values to give to the job</param>
        /// <returns>The output values</returns>
        IEnumerable<IProcessJobField> ProcessJob(IEnumerable<IProcessJobField> inputValues = null);

        /// <summary>
        /// Create and process a job using the given input values asynchronously
        /// </summary>
        /// <param name="inputValues">The input values to give to the job</param>
        /// <returns>The task to invoke</returns>
        Task<IEnumerable<IProcessJobField>> ProcessJobAsync(IEnumerable<IProcessJobField> inputValues = null);

        /// <summary>
        /// Gets the descriptor that holds the core details of the job
        /// </summary>
        IJobDescriptor JobDescriptor { get; }

        /// <summary>
        /// Gets the plugin descriptor for the process
        /// </summary>
        IPluginDescriptor PluginDescriptor { get; }

        IReadOnlyList<string> RegisteredPluginDependencyAssemblies { get; }
    }
}
