using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// Interface for a plugin assembly factory
    /// </summary>
    public interface IPluginAssemblyFactory
    {
        /// <summary>
        /// Creates a plugin factory from a given .NET assembly at a file path
        /// </summary>
        /// <param name="netAssemblyPath">The path to the .NET assembly to use</param>
        /// <returns>The plugin assembly</returns>
        IPluginAssembly CreatePluginAssemblyFromPath(string netAssemblyPath);
    }
}
