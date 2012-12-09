using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginBootstrapServiceFactory : IPluginBootstrapServiceFactory
    {
        private IKernel _kernel;

        public PluginBootstrapServiceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginBootstrapService CreateService()
        {
            return _kernel.Get<IPluginBootstrapService>();
        }
    }
}
