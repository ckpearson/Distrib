using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// Factory class for creating plugin assemblies
    /// </summary>
    public sealed class PluginAssemblyFactory : IPluginAssemblyFactory
    {
        private IKernel _kernel;

        public PluginAssemblyFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IPluginAssembly CreatePluginAssemblyFromPath(string netAssemblyPath)
        {
            return _kernel.Get<IPluginAssembly>(new[]
            {
                new ConstructorArgument("netAssemblyPath", netAssemblyPath),
            });
        }
    }
}
