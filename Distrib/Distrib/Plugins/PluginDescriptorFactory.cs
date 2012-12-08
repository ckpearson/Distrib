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
    public sealed class PluginDescriptorFactory : MarshalByRefObject, IPluginDescriptorFactory
    {
        private readonly IKernel _kernel;

        public PluginDescriptorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginDescriptor GetDescriptor(string typeFullName, IPluginMetadata pluginMetadata)
        {
            return _kernel.Get<IPluginDescriptor>(new[]
            {
                new ConstructorArgument("kernel",
                    _kernel.Get<IRemoteKernelFactory>()
                        .GetRemoteKernel(_kernel)),
                new ConstructorArgument("typeFullName", typeFullName),
                new ConstructorArgument("metadata", pluginMetadata),
            });
        }
    }
}
