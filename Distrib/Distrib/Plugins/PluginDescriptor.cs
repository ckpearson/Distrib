using Distrib.IOC;
using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    public sealed class PluginDescriptor : IPluginDescriptor
    {
        private readonly string _typeFullName;
        private readonly IPluginMetadata _metadata;

        private readonly WriteOnce<bool> _pluginFoundUsable = new WriteOnce<bool>(false);
        private readonly WriteOnce<PluginExclusionReason> _pluginExclusionReason =
            new WriteOnce<PluginExclusionReason>(PluginExclusionReason.Unknown);
        private readonly WriteOnce<object> _pluginExclusionTag =
            new WriteOnce<object>(null);

        private readonly WriteOnce<IReadOnlyList<IPluginMetadataBundle>> _additionalMetadata =
            new WriteOnce<IReadOnlyList<IPluginMetadataBundle>>(null);

        private readonly object _lock = new object();

        public PluginDescriptor(string typeFullName, IPluginMetadata metadata)
        {
            _typeFullName = typeFullName;
            _metadata = metadata;
        }

        public string PluginTypeName
        {
            get { return _typeFullName; }
        }

        public IPluginMetadata Metadata
        {
            get { return _metadata; }
        }

        public IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles
        {
            get
            {
                lock (_additionalMetadata)
                {
                    return (!_additionalMetadata.IsWritten) ? null : _additionalMetadata.Value;
                }
            }
        }

        public void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additionalMetadataBundles)
        {
            lock (_additionalMetadata)
            {
                if (!_additionalMetadata.IsWritten)
                {
                    _additionalMetadata.Value = additionalMetadataBundles.ToList().AsReadOnly();
                }
                else
                {
                    throw new InvalidOperationException("Additional metadata already set");
                }
            }
        }

        private T _UsabilitySetRequiredReturn<T>(Func<T> func)
        {
            lock (_lock)
            {
                if (!UsabilitySet)
                {
                    throw new InvalidOperationException("Usability not set yet");
                }
                else
                {
                    return func();
                }
            }
        }

        public bool IsUsable
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginFoundUsable.Value);
            }
        }

        public PluginExclusionReason ExlusionReason
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginExclusionReason.Value);
            }
        }

        public object ExclusionTag
        {
            get
            {
                return _UsabilitySetRequiredReturn(() => _pluginExclusionTag.Value);
            }
        }

        public void MarkAsUsable()
        {
            lock (_lock)
            {
                if (!UsabilitySet)
                {
                    _pluginFoundUsable.Value = true;
                    _pluginExclusionReason.Value = PluginExclusionReason.Unknown;
                    _pluginExclusionTag.Value = null;
                }
                else
                {
                    throw new InvalidOperationException("Usability has already been set");
                }
            }
        }

        public void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null)
        {
            lock (_lock)
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
                lock (_lock)
                {
                    return _pluginFoundUsable.IsWritten;
                }
            }
        }
    }
}
