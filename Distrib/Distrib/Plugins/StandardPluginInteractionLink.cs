using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    internal sealed class StandardPluginInteractionLink : CrossAppDomainObject, IPluginInteractionLink
    {
        private readonly IPluginDescriptor _pluginDescriptor;
        private readonly IPlugin _pluginRawInstance;
        private readonly IPluginController _pluginController;
        private readonly IPluginInstance _pluginManagedInstance;

        private readonly object _lock = new object();

        public StandardPluginInteractionLink(IPluginDescriptor pluginDescriptor, IPlugin pluginRawInstance, 
            IPluginController pluginController,
            IPluginInstance pluginManagedInstance)
        {
            _pluginDescriptor = pluginDescriptor;
            _pluginRawInstance = pluginRawInstance;
            _pluginController = pluginController;
            _pluginManagedInstance = pluginManagedInstance;
        }

        public IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles
        {
            get
            {
                lock (_lock)
                {
                    return _pluginDescriptor.AdditionalMetadataBundles;
                }
            }
        }

        public DateTime PluginCreationStamp
        {
            get
            {
                lock (_lock)
                {
                    return _pluginManagedInstance.InstanceCreationStamp;
                }
            }
        }

        public IPluginMetadata PluginMetadata
        {
            get
            {
                lock (_lock)
                {
                    return _pluginDescriptor.Metadata;
                }
            }
        }
    }
}
