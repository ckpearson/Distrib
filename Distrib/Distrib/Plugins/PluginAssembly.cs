using Distrib.Separation;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssembly : IPluginAssembly
    {
        private readonly string _netAssemblyPath;
        private readonly IKernel _kernel;

        private AppDomain _appDomain;
        private IPluginAssemblyManager _assemblyManager;

        public PluginAssembly(IKernel kernel, string netAssemblyPath)
        {
            if (string.IsNullOrEmpty(netAssemblyPath)) throw new ArgumentNullException("Assembly path must be supplied");

            _kernel = kernel;
            _netAssemblyPath = netAssemblyPath;
        }

        public void Initialise()
        {
            _appDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + _netAssemblyPath);

            try
            {
                _assemblyManager = _kernel.Get<IPluginAssemblyManagerFactory>().CreateManagerForAssemblyInGivenDomain
                    (_netAssemblyPath, _appDomain);
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public void Unitialise()
        {
            throw new NotImplementedException();
        }
    }
}
