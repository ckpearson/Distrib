using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Factory for creating field configs
    /// </summary>
    internal static class ProcessJobFieldConfigFactory
    {
        /// <summary>
        /// Create a non-generic config
        /// </summary>
        /// <returns>The config</returns>
        public static IProcessJobFieldConfig CreateConfig()
        {
            return new ProcessJobFieldConfig();
        }
        
        /// <summary>
        /// Create a generic config
        /// </summary>
        /// <typeparam name="T">The config type</typeparam>
        /// <returns>The config</returns>
        public static IProcessJobFieldConfig<T> CreateConfig<T>()
        {
            return new ProcessJobFieldConfig<T>();
        }
    }
}
