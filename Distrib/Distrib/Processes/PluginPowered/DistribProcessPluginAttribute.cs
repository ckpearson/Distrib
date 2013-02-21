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

namespace Distrib.Processes.PluginPowered
{
    /// <summary>
    /// Indicates that a given class is to be treated as a Distrib Process Plugin
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DistribProcessPluginAttribute : PluginAttribute
    {
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="name">The name of the process</param>
        /// <param name="description">The description of the process</param>
        /// <param name="version">The version of the process</param>
        /// <param name="author">The author of the process</param>
        public DistribProcessPluginAttribute(string name,
            string description,
            double version,
            string author,
            string identifier) : base(typeof(IProcess), name, description, version, author, identifier)
        {
            base.SuppliedMetadataObjects = new List<PluginAdditionalMetadataObject>()
            {
                new ProcessMetadataObject(name, description, version, author),
            }.AsReadOnly();
        }
    }
}
