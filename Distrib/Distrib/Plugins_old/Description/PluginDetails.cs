using Distrib.Plugins_old.Discovery;
using Distrib.Plugins_old.Discovery.Metadata;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old.Description
{
    [Serializable()]
    public sealed class PluginDetails : IPluginDetails
    {
        private readonly string _pluginTypeName;
        private readonly IPluginMetadata _pluginMetadata;

        private readonly WriteOnce<bool> _pluginFoundUsable = new WriteOnce<bool>(false);
        private readonly WriteOnce<PluginExclusionReason> _pluginExclusionReason = new WriteOnce<PluginExclusionReason>(PluginExclusionReason.Unknown);
        private readonly WriteOnce<object> _pluginExclusionTag = new WriteOnce<object>(null);

        private readonly WriteOnce<IReadOnlyList<IPluginAdditionalMetadataBundle>> _additionalMetadata =
            new WriteOnce<IReadOnlyList<IPluginAdditionalMetadataBundle>>(null);

        private readonly object m_lock = new object();

        public PluginDetails(string typeName, IPluginMetadata metadata)
        {
            _pluginTypeName = typeName;
            _pluginMetadata = metadata;
        }

        public string PluginTypeName
        {
            get { return _pluginTypeName; }
        }

        public IPluginMetadata Metadata
        {
            get { return _pluginMetadata; }
        }

        public IReadOnlyList<IPluginAdditionalMetadataBundle> AdditionalMetadataBundles
        {
            get
            {
                lock (_additionalMetadata)
                {
                    return _additionalMetadata.Value.NullIfSo();
                }
            }
        }

        public void SetAdditionalMetadata(IEnumerable<IPluginAdditionalMetadataBundle> additionalMetadata)
        {
            lock (_additionalMetadata)
            {
                if (!_additionalMetadata.IsWritten)
                {
                    _additionalMetadata.Value = additionalMetadata.ToList().AsReadOnly();
                }
                else
                {
                    throw new InvalidOperationException("Additional metadata already set");
                }
            }
        }

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
                        return _pluginFoundUsable.Value;
                    }
                }
            }
        }

        public PluginExclusionReason ExclusionReason
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
                        return _pluginExclusionReason.Value;
                    }
                }
            }
        }

        public object ExclusionTag
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
                        return _pluginExclusionTag.Value;
                    }
                }
            }
        }

        public void MarkAsUsable()
        {
            lock (m_lock)
            {
                if (!UsabilitySet)
                {
                    _pluginFoundUsable.Value = true;
                    _pluginExclusionReason.Value = PluginExclusionReason.Unknown;
                }
                else
                {
                    throw new InvalidOperationException("Usability has already been set");
                }
            }
        }

        public void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null)
        {
            lock (m_lock)
            {
                if (!UsabilitySet)
                {
                    _pluginFoundUsable.Value = false;
                    _pluginExclusionReason.Value = exclusionReason;
                    _pluginExclusionTag.Value = tag;
                }
                else
                {
                    throw new InvalidOperationException("Usability has already been set");
                }
            }
        }

        public bool UsabilitySet
        {
            get
            {
                lock (m_lock)
                {
                    return _pluginFoundUsable.IsWritten;
                }
            }
        }
    }
}
