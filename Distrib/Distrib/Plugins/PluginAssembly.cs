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

        public IPluginAssemblyInitialisationResult Initialise()
        {
            List<IPluginDescriptor> lstPluginDescriptorsForResult = new List<IPluginDescriptor>();

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

                    if (_pluginDescriptors == null || _pluginDescriptors.Count == 0)
                    {
                        throw new InvalidOperationException("Plugin assembly contains no plugins");
                    }

                    foreach (IPluginDescriptor descriptor in _pluginDescriptors)
                    {
                        // Perform the bootstrapping for the plugin
                        var bootstrapResult = _kernel.Get<IPluginBootstrapServiceFactory>()
                            .CreateService()
                            .BootstrapPlugin(descriptor);

                        if (!bootstrapResult.Success)
                        {
                            descriptor.MarkAsUnusable(PluginExclusionReason.PluginBootstrapFailure,
                                bootstrapResult.Result);

                            lstPluginDescriptorsForResult.Add(descriptor);

                            continue;
                        }

                        // Made it this far so mark as usable
                        descriptor.MarkAsUsable();
                        lstPluginDescriptorsForResult.Add(descriptor);
                    }

                    return _kernel.Get<IPluginAssemblyInitialisationResultFactory>()
                        .CreateResultFromPlugins(lstPluginDescriptorsForResult.AsReadOnly());
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
