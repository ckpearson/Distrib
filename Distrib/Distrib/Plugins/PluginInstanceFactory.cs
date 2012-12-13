using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginInstanceFactory : IPluginInstanceFactory
    {
        private readonly IKernel _kernel;

        public PluginInstanceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginInstance CreatePluginInstance(IPluginDescriptor descriptor, IPluginAssembly parentAssembly)
        {
            return _kernel.Get<IPluginInstance>(new[]
            {
                new ConstructorArgument("descriptor", descriptor),
                new ConstructorArgument("pluginAssembly", parentAssembly),
            });
        }
    }
}
