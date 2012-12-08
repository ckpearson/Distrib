using Distrib.IOC;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssemblyManager : MarshalByRefObject, IPluginAssemblyManager
    {
        private readonly IRemoteKernel _kernel;
        private readonly string _assemblyPath;

        public PluginAssemblyManager(IRemoteKernel kernel, string assemblyPath)
        {
            _kernel = kernel;
            _assemblyPath = assemblyPath;
        }

        public void LoadPluginAssemblyIntoDomain()
        {
            throw new NotImplementedException();
        }

        public object CreateInstanceFromPluginAssembly()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IPluginDescriptor> GetPluginDescriptors()
        {
            throw new NotImplementedException();
        }

        public bool PluginTypeAdheresToPluginInterface(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool PluginTypeImplementsCorePluginInterface(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool PluginTypeIsMarshalable(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
