using Distrib.Plugins_old.Discovery;
using Distrib.Plugins_old.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old.Controllers
{
    public interface IDistribPluginControllerInterface
    {
        /// <summary>
        /// Gets the core metadata for the plugin
        /// </summary>
        DistribPluginMetadata PluginMetadata { get; }

        IReadOnlyList<IPluginAdditionalMetadataBundle> AdditionalMetadata { get; }
    }
}
