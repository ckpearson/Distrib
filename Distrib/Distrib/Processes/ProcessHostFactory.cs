using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHostFactory : IProcessHostFactory
    {
        private readonly IKernel _kernel;

        public ProcessHostFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IProcessHost CreateHostForProcessPlugin(Plugins.IPluginInstance pluginInstance)
        {
            return _kernel.Get<IProcessHost>(new[]
            {
                new ConstructorArgument("pluginInstance", pluginInstance),
            });
        }
    }
}
