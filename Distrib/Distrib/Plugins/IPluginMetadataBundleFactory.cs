using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginMetadataBundleFactory
    {
        IPluginMetadataBundle CreateBundle(Type interfaceType,
            Type attributeType,
            object instance,
            IReadOnlyDictionary<string, object> kvps,
            string identity,
            PluginMetadataBundleExistencePolicy existencePolicy);
    }
}
