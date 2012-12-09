using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginControllerFactory : IPluginControllerFactory
    {
        private IKernel _kernel;

        public PluginControllerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginController CreateController()
        {
            return _kernel.Get<IPluginController>();
        }
    }
}
