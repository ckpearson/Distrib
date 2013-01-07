using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginDescriptorPersistorFactory : IPluginDescriptorPersistorFactory
    {
        private readonly IKernel _kernel;

        public PluginDescriptorPersistorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginDescriptorPersistor Create()
        {
            return _kernel.Get<IPluginDescriptorPersistor>();
        }
    }
}
