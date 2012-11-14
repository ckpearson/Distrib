using Distrib.Plugins.Discovery;
using Distrib.Plugins.Discovery.Metadata;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Description
{
    /// <summary>
    /// Represents the details of a distrib plugin required for construction by a plugin assembly
    /// </summary>
    [Serializable()]
    public sealed class DistribPluginDetails
    {
        private readonly string m_strTypeName = "";
        private readonly DistribPluginMetadata m_metadata = null;

        private readonly WriteOnce<bool> m_bPluginFoundToBeUsable = new WriteOnce<bool>(false);
        private readonly WriteOnce<DistribPluginExlusionReason> m_pluginExclusionReason =
            new WriteOnce<DistribPluginExlusionReason>(DistribPluginExlusionReason.Unknown);

        private readonly WriteOnce<List<IDistribPluginAdditionalMetadataBundle>>
            m_lstAdditionalMetadata = new WriteOnce<List<IDistribPluginAdditionalMetadataBundle>>(null);

        private readonly object m_lock = new object();

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="pluginTypeName">The full name of the type within the assembly this plugin is for</param>
        /// <param name="metadata">The metadata of the plugin</param>
        internal DistribPluginDetails(string pluginTypeName, DistribPluginMetadata metadata)
        {
            m_strTypeName = pluginTypeName;
            m_metadata = metadata;
        }

        /// <summary>
        /// Gets the full name of the type the plugin uses
        /// </summary>
        public string PluginTypeName
        {
            get { return m_strTypeName; }
        }

        /// <summary>
        /// Gets the metadata for the plugin
        /// </summary>
        public DistribPluginMetadata Metadata
        {
            get { return m_metadata; }
        }

        public List<IDistribPluginAdditionalMetadataBundle> AdditionalMetadataBundles
        {
            get
            {
                lock (m_lstAdditionalMetadata)
                {
                    return (!m_lstAdditionalMetadata.IsWritten) ? null :
                                m_lstAdditionalMetadata.Value;
                }
            }

            internal set
            {
                lock (m_lstAdditionalMetadata)
                {
                    if (!m_lstAdditionalMetadata.IsWritten)
                    {
                        m_lstAdditionalMetadata.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Additional metadata already set");
                    }
                }
            }
        }

        /// <summary>
        /// Gets whether the plugin has been marked as usable
        /// </summary>
        public bool IsUsable
        {
            get
            {
                lock (m_lock)
                {
                    if (!UsabilitySet)
                    {
                        throw new InvalidOperationException("Usability not set yet");
                    }
                    else
                    {
                        return m_bPluginFoundToBeUsable.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the reason the plugin was excluded from being used
        /// </summary>
        public DistribPluginExlusionReason ExclusionReason
        {
            get
            {
                lock (m_lock)
                {
                    if (!UsabilitySet)
                    {
                        throw new InvalidOperationException("Usability not set yet");
                    }
                    else
                    {
                        return m_pluginExclusionReason.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Marks the plugin as being usable by the system as it passed discovery-time validation
        /// </summary>
        internal void MarkAsUsable()
        {
            lock (m_lock)
            {
                m_bPluginFoundToBeUsable.Value = true;
                m_pluginExclusionReason.Value = DistribPluginExlusionReason.Unknown;
            }
        }

        /// <summary>
        /// Marks the plugin as being unusable by the system as it failed discovery-time validation
        /// </summary>
        /// <param name="exclusionReason">The reason why the plugin is being excluded from the system</param>
        internal void MarkAsUnusable(DistribPluginExlusionReason exclusionReason)
        {
            lock (m_lock)
            {
                m_bPluginFoundToBeUsable.Value = false;
                m_pluginExclusionReason.Value = exclusionReason;
            }
        }

        /// <summary>
        /// Gets whether the usability details for the plugin have been set
        /// </summary>
        internal bool UsabilitySet
        {
            get
            {
                lock (m_lock)
                {
                    return m_bPluginFoundToBeUsable.IsWritten;
                }
            }
        }
    }
}
