using Distrib.IOC;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssemblyManagerFactory : IPluginAssemblyManagerFactory
    {
        private readonly IKernel _kernel;
        private readonly IRemoteKernel _remoteKernel;

        public PluginAssemblyManagerFactory(IKernel kernel)
        {
            _kernel = kernel;

            // Set up the remote kernel using the current kernel
            _remoteKernel = _kernel.Get<IRemoteKernelFactory>().GetRemoteKernel(_kernel);
        }

        public IPluginAssemblyManager CreateManagerForAssembly(string assemblyPath)
        {
            return _kernel.Get<IPluginAssemblyManager>(new[]
            {
                new ConstructorArgument("kernel", _remoteKernel),
                new ConstructorArgument("assemblyPath", assemblyPath),
            });
        }

        public IPluginAssemblyManager CreateManagerForAssemblyInGivenDomain(string assemblyPath, AppDomain domain)
        {
            var t = this.CreateManagerForAssembly(assemblyPath).GetType();

            return (IPluginAssemblyManager)domain.CreateInstanceAndUnwrap(
                t.Assembly.FullName,
                t.FullName,
                true,
                System.Reflection.BindingFlags.CreateInstance,
                null,
                new object[] { _remoteKernel, assemblyPath },
                null,
                null);
        }
    }
}
