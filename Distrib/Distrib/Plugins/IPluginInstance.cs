using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginInstance
    {
        /// <summary>
        /// Gets the unique identifier for this instance of the plugin
        /// </summary>
        string InstanceID { get; }

        /// <summary>
        /// Gets the date-time stamp for when this plugin's underlying instance was actually created
        /// </summary>
        DateTime InstanceCreationStamp { get; }

        /// <summary>
        /// Lazily gets the underlying plugin instance (the object itself) cast in the form desired
        /// </summary>
        /// <typeparam name="T">The type to cast the underlying instance to</typeparam>
        /// <returns>The underlying plugin instance</returns>
        T GetUnderlyingInstance<T>() where T : class;

        /// <summary>
        /// Initialises the plugin instance
        /// </summary>
        void Initialise();

        /// <summary>
        /// Gets whether the plugin instance has been initialised
        /// </summary>
        bool IsInitialised { get; }

        /// <summary>
        /// Unitialises the plugin instance
        /// </summary>
        void Unitialise();

        IPluginDescriptor PluginDescriptor { get; }

        IPluginAssembly SpawningAssembly { get; }

        IReadOnlyList<string> DeclaredAssemblyDependencies { get; }
    }
}
