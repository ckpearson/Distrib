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

        void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additonalMetadata);
    }

    public interface ICheckedPluginDescriptor : IPluginDescriptor
    {
        bool IsUsable { get; }

        PluginExclusionReason ExclusionReason { get; }

        object ExclusionTag { get; }

        void MarkAsUsable();
        void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null);
    }
}
