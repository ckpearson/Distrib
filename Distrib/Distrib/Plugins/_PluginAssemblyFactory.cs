using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class _PluginAssemblyFactory : _IPluginAssemblyFactory
    {
        private readonly IKernel _kernel;

        public _PluginAssemblyFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public _IPluginAssembly FromPath(string assemblyPath)
        {
            return _kernel.Get<_IPluginAssembly>(new[]
            {
                new ConstructorArgument("assemblyPath", assemblyPath),
            });
        }
    }
}
