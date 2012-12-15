using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginInteractionLinkFactory
    {
        IPluginInteractionLink CreateInteractionLink(IPluginDescriptor pluginDescriptor,
            IPlugin pluginRawInstance,
            IPluginController pluginController,
            IPluginInstance pluginManagedInstance);
    }
}
