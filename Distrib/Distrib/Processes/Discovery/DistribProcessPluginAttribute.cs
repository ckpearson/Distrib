using Distrib.Plugins;
using Distrib.Plugins_old.Discovery;
using Distrib.Processes.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Discovery
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
            string identifier) : base(typeof(IDistribProcess), name, description, version, author, identifier)
        {
            base.SuppliedMetadataObjects = new List<PluginAdditionalMetadataObject>()
            {
#warning Process plugin attribute needs additional metadata attributes upgrading
                new ProcessMetadataObject(name, description, version, author),
            }.AsReadOnly();
        }
    }
}
