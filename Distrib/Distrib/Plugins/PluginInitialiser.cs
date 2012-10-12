using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// Class for remotely loading assemblies into plugin domains
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is simply instantiated within the plugin domain and then called to load
    /// the plugin assembly into that domain.
    /// </para>
    /// </remarks>
    internal class PluginInitialiser : MarshalByRefObject
    {
        /// <summary>
        /// Loads the given assembly
        /// </summary>
        /// <param name="assemblyPath">The path of the assembly to load</param>
        public void LoadAssemblyIntoPlugin(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentException("Assembly path cannot be null or empty");

            try
            {
                Assembly.LoadFile(assemblyPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load assembly into plugin", ex);
            }
        }
    }
}
