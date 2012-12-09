using Distrib.IOC;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginMetadataBundleFactory : MarshalByRefObject, IPluginMetadataBundleFactory
    {
        private IKernel _kernel;

        public PluginMetadataBundleFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginMetadataBundle CreateBundle(Type interfaceType,
            object instance, 
            IReadOnlyDictionary<string, object> kvps, 
            string identity, 
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            return _kernel.Get<IPluginMetadataBundle>(new[]
            {
                new ConstructorArgument("interfaceType", interfaceType),
                new ConstructorArgument("instance", instance),
                new ConstructorArgument("kvps", kvps),
                new ConstructorArgument("identity", identity),
                new ConstructorArgument("existencePolicy", existencePolicy),
            });
        }
    }
}
