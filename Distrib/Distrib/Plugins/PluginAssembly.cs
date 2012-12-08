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

        private readonly object _lock = new object();

        private bool _isInitialised = false;

        private IReadOnlyList<IPluginDescriptor> _pluginDescriptors;

        public PluginAssembly(IKernel kernel, string netAssemblyPath)
        {
            if (string.IsNullOrEmpty(netAssemblyPath)) throw new ArgumentNullException("Assembly path must be supplied");

            _kernel = kernel;
            _netAssemblyPath = netAssemblyPath;
        }

        public void Initialise()
        {
            try
            {
                lock (_lock)
                {
                    if (IsInitialised)
                    {
                        throw new InvalidOperationException("Plugin assembly already initialised");
                    }

                    // Set up the AppDomain
                    _appDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + _netAssemblyPath);

                    try
                    {
                        // Create the assembly manager
                        _assemblyManager =
                            _kernel.Get<IPluginAssemblyManagerFactory>()
                                .CreateManagerForAssemblyInGivenDomain(
                                    _netAssemblyPath,
                                    _appDomain);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create assembly manager", ex);
                    }

                    // Need to make sure a manager is present
                    if (_assemblyManager == null)
                    {
                        throw new ApplicationException("Manager was null after attempted creation");
                    }

                    try
                    {
                        // Load the assembly into the domain
                        _assemblyManager.LoadPluginAssemblyIntoDomain();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to load assembly into AppDomain", ex);
                    }

                    // Pull out the plugin descriptors
                    _pluginDescriptors = _assemblyManager.GetPluginDescriptors();
                }
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


        public bool IsInitialised
        {
            get
            {
                lock (_lock)
                {
                    return _isInitialised;
                }
            }
        }
    }
}
