using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssemblyInitialisationResultFactory : IPluginAssemblyInitialisationResultFactory
    {
        private IKernel _kernel;

        public PluginAssemblyInitialisationResultFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        IPluginAssemblyInitialisationResult IPluginAssemblyInitialisationResultFactory.CreateResultFromPlugins(
            IReadOnlyList<IPluginDescriptor> descriptorList)
        {
            return _kernel.Get<IPluginAssemblyInitialisationResult>(new[]
            {
                new ConstructorArgument("descriptorList", descriptorList),
            });
        }
    }
}
