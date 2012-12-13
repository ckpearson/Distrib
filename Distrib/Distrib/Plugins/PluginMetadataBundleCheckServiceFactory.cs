using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginMetadataBundleCheckServiceFactory : IPluginMetadataBundleCheckServiceFactory
    {
        private readonly IKernel _kernel;

        public PluginMetadataBundleCheckServiceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginMetadataBundleCheckService CreateService()
        {
            return _kernel.Get<IPluginMetadataBundleCheckService>();
        }
    }
}
