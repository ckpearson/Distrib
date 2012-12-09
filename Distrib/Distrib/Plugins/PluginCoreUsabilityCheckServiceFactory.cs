using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginCoreUsabilityCheckServiceFactory : IPluginCoreUsabilityCheckServiceFactory
    {
        private IKernel _kernel;

        public PluginCoreUsabilityCheckServiceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginCoreUsabilityCheckService CreateService()
        {
            return _kernel.Get<IPluginCoreUsabilityCheckService>();
        }
    }
}
