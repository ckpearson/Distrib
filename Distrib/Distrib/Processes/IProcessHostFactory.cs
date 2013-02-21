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
        //IProcessHost CreateHostFromPluginSeparated(IPluginDescriptor descriptor);

        IProcessHost CreateHostFromType(Type type);
        //IProcessHost CreateHostFromTypeSeparated(Type type);
    }
}
