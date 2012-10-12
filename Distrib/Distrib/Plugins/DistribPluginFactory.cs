using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// Factory for creating plugins
    /// </summary>
    public static class DistribPluginFactory
    {
        /// <summary>
        /// Creates a plugin from a given assembly
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly</param>
        /// <returns>The <see cref="DistribPlugin"/> representing the plugin assembly</returns>
        public static DistribPlugin PluginFromAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("Assembly path must be supplied");
            if (!File.Exists(assemblyPath)) throw new FileNotFoundException("Assembly file not found");

            try
            {
                var appdomain = AppDomain.CreateDomain(Guid.NewGuid().ToString() + "_" + assemblyPath);
                return new DistribPlugin(appdomain, assemblyPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin from assembly file", ex);
            }
        }
    }
}
