using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginInteractionLinkFactory : CrossAppDomainObject, IPluginInteractionLinkFactory
    {
        public IPluginInteractionLink CreateInteractionLink(IPluginDescriptor pluginDescriptor, 
            IPlugin pluginRawInstance, IPluginController pluginController, IPluginInstance pluginManagedInstance)
        {
            return new StandardPluginInteractionLink(pluginDescriptor,
                pluginRawInstance,
                pluginController,
                pluginManagedInstance);
        }
    }
}
