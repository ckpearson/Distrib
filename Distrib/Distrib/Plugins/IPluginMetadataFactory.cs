using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginMetadataFactory
    {
        IPluginMetadata CreateMetadata(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            string identifier,
            Type controllerType);

        IPluginMetadata CreateMetadataFromPluginAttribute(PluginAttribute attribute);
    }
}
