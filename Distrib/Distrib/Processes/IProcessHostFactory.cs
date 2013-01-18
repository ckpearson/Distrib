using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Factory for creating a process host
    /// </summary>
    public interface IProcessHostFactory
    {
        /// <summary>
        /// Create a process host for the given plugin
        /// </summary>
        /// <param name="descriptor">The descriptor for the plugin to use</param>
        /// <returns>The created process host</returns>
        IProcessHost CreateHostFromPlugin(IPluginDescriptor descriptor);

        /// <summary>
        /// Create a process host for the given plugin, but separate it to its own AppDomain
        /// </summary>
        /// <param name="descriptor">The descriptor for the plugin to use</param>
        /// <returns>The created process host</returns>
        IProcessHost CreateHostFromPluginSeparated(IPluginDescriptor descriptor);
    }
}
