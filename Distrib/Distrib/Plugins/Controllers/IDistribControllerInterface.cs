using Distrib.Plugins.Discovery;
using Distrib.Plugins.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Controllers
{
    public interface IDistribPluginControllerInterface
    {
        /// <summary>
        /// Gets the core metadata for the plugin
        /// </summary>
        DistribPluginMetadata PluginMetadata { get; }

        IReadOnlyList<IDistribPluginAdditionalMetadataBundle> AdditionalMetadata { get; }
    }
}
