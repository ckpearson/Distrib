using Distrib.Plugins_old.Discovery;
using Distrib.Plugins_old.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old.Description
{
    /// <summary>
    /// Interface for a plugins core details
    /// </summary>
    public interface IPluginDetails
    {
        /// <summary>
        /// Gets the name of the type used by the plugin
        /// </summary>
        string PluginTypeName { get; }

        /// <summary>
        /// Gets the metadata for the plugin
        /// </summary>
        IPluginMetadata Metadata { get; }

        /// <summary>
        /// Gets the additional metadata bundles for the plugin
        /// </summary>
        IReadOnlyList<IPluginAdditionalMetadataBundle> AdditionalMetadataBundles { get; }

        /// <summary>
        /// Sets the additional metadata bundles for the plugin
        /// </summary>
        /// <param name="additionalMetadataBundles">The additional metadata bundles for the plugin</param>
        void SetAdditionalMetadata(IEnumerable<IPluginAdditionalMetadataBundle> additionalMetadataBundles);

        /// <summary>
        /// Gets whether the plugin has been found to be usable
        /// </summary>
        bool IsUsable { get; }

        /// <summary>
        /// Gets the reason why the plugin was excluded from use (if it was)
        /// </summary>
        PluginExclusionReason ExclusionReason { get; }

        /// <summary>
        /// Gets the accompanying tag that may be set in the event the plugin was found unusable
        /// </summary>
        object ExclusionTag { get; }

        /// <summary>
        /// Irrevocably marks a plugin as being usable
        /// </summary>
        void MarkAsUsable();

        /// <summary>
        /// Irrevocably marks a plugin as being unusable
        /// </summary>
        /// <param name="exclusionReason"></param>
        /// <param name="tag"></param>
        void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null);

        bool UsabilitySet { get; }
    }
}
