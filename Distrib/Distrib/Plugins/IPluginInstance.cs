/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
