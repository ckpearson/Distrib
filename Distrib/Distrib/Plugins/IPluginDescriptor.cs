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

        string AssemblyPath { get; }

        IPluginMetadata Metadata { get; }

        IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles { get; }

        void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additionalMetadataBundles);

        bool IsUsable { get; }

        PluginExclusionReason ExclusionReason { get; }

        object ExclusionTag { get; }

        void MarkAsUsable();

        void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null);

        bool UsabilitySet { get; }

        /// <summary>
        /// Determines whether a given <see cref="IPluginDescriptor"/> describes the same fundamental
        /// plugin as the current one (no check into usability / metadata)
        /// </summary>
        /// <param name="descriptor">The descriptor to check against</param>
        /// <returns><c>True</c> if it is the same plugin, <c>False</c> otherwise</returns>
        bool Match(IPluginDescriptor descriptor);
    }
}
