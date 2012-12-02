using Distrib.Plugins.Discovery;
using Distrib.Plugins.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Description
{
    public interface IPluginDetails
    {
        string PluginTypeName { get; }
        IPluginMetadata Metadata { get; }

        IReadOnlyList<IPluginAdditionalMetadataBundle> AdditionalMetadataBundles { get; }
        void SetAdditionalMetadata(IEnumerable<IPluginAdditionalMetadataBundle> additionalMetadata);

        bool IsUsable { get; }
        PluginExclusionReason ExclusionReason { get; }
        object ExclusionTag { get; }

        void MarkAsUsable();
        void MarkAsUnusable(PluginExclusionReason exclusionReason,
            object tag = null);

        bool UsabilitySet { get; }
    }
}
