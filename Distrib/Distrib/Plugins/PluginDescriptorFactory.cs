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
    public sealed class PluginDescriptorFactory : CrossAppDomainObject, IPluginDescriptorFactory
    {
        private readonly IKernel _kernel;

        public PluginDescriptorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginDescriptor GetDescriptor(string typeFullName, IPluginMetadata pluginMetadata, 
            string assemblyPath)
        {
            return _kernel.Get<IPluginDescriptor>(new[]
            {
                new ConstructorArgument("typeFullName", typeFullName),
                new ConstructorArgument("metadata", pluginMetadata),
                new ConstructorArgument("assemblyPath", assemblyPath),
            });
        }
    }
}
