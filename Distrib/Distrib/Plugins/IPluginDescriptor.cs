using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginDescriptor
    {
        string PluginTypeName { get; }

        IPluginMetadata Metadata { get; }

        IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles { get; }

        void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additionalMetadataBundles);

        bool IsUsable { get; }

        object ExlusionReason { get; }

        object ExclusionTag { get; }

        void MarkAsUsable();

        void MarkAsUnusable();

        bool UsabilitySet { get; }
    }
}
