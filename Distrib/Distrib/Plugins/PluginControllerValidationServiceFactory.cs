using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginControllerValidationServiceFactory : IPluginControllerValidationServiceFactory
    {
        private IKernel _kernel;

        public PluginControllerValidationServiceFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginControllerValidationService CreateService()
        {
            return _kernel.Get<IPluginControllerValidationService>();
        }
    }
}
