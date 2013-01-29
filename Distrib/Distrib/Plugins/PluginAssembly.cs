/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssembly : CrossAppDomainObject, IPluginAssembly
    {
        private readonly string _netAssemblyPath;

        private AppDomain _appDomain;
        private IPluginAssemblyManager _assemblyManager;

        private readonly IPluginAssemblyManagerFactory _asmManagerFactory;
        private readonly IPluginBootstrapService _pluginBootstrapService;
        private readonly IPluginCoreUsabilityCheckService _pluginCoreUsabilityCheckService;
        private readonly IPluginControllerValidationService _pluginControllerValidationService;
        private readonly IPluginMetadataBundleCheckService _pluginMetadataBundleCheckService;
        private readonly IPluginAssemblyInitialisationResultFactory _pluginAsmInitialisationResultFactory;

        private readonly IPluginInstanceFactory _pluginInstanceFactory;

        private readonly object _lock = new object();

        private bool _isInitialised = false;

        private IReadOnlyList<IPluginDescriptor> _pluginDescriptors;

        public PluginAssembly(IPluginAssemblyManagerFactory asmManagerFactory, 
            IPluginBootstrapService pluginBootstrapService,
            IPluginCoreUsabilityCheckService pluginCoreUsabilityCheckService,
            IPluginControllerValidationService pluginControllerValidationService,
            IPluginMetadataBundleCheckService pluginMetadataCheckService,
            IPluginAssemblyInitialisationResultFactory pluginAssemblyInitialisationResultFactory,
            IPluginInstanceFactory pluginInstanceFactory,
            string netAssemblyPath)
        {
            if (string.IsNullOrEmpty(netAssemblyPath)) throw new ArgumentNullException("Assembly path must be supplied");

            _asmManagerFactory = asmManagerFactory;
            _pluginBootstrapService = pluginBootstrapService;
            _pluginCoreUsabilityCheckService = pluginCoreUsabilityCheckService;
            _pluginControllerValidationService = pluginControllerValidationService;
            _pluginMetadataBundleCheckService = pluginMetadataCheckService;
            _pluginAsmInitialisationResultFactory = pluginAssemblyInitialisationResultFactory;

            _pluginInstanceFactory = pluginInstanceFactory;

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
                        _assemblyManager = _asmManagerFactory.CreateManagerForAssemblyInGivenDomain(
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
                        var bootstrapResult = _pluginBootstrapService.BootstrapPlugin(descriptor);

                        if (!bootstrapResult.Success)
                        {
                            descriptor.MarkAsUnusable(PluginExclusionReason.PluginBootstrapFailure,
                                bootstrapResult.Result);

                            lstPluginDescriptorsForResult.Add(descriptor);

                            continue;
                        }

                        // Perform the core usability checking
                        var usabilityCheckResult = _pluginCoreUsabilityCheckService
                            .CheckCoreUsability(descriptor, _assemblyManager);

                        if (!usabilityCheckResult.Success)
                        {
                            descriptor.MarkAsUnusable(usabilityCheckResult.Result,
                                usabilityCheckResult.ResultTwo);

                            lstPluginDescriptorsForResult.Add(descriptor);

                            continue;
                        }

                        // Perform the verification for usability of the plugin controller
                        var controllerValidationResult = _pluginControllerValidationService
                            .ValidateControllerType(descriptor.Metadata.ControllerType);

                        if (!controllerValidationResult.Success)
                        {
                            descriptor.MarkAsUnusable(PluginExclusionReason.PluginControllerInvalid,
                                controllerValidationResult.ResultTwo);

                            lstPluginDescriptorsForResult.Add(descriptor);

                            continue;
                        }

                        // Make sure the additional metadata bundles adhere to their constraints
                        var metadataConstraintsCheckResult = _pluginMetadataBundleCheckService
                            .CheckMetadataBundlesFulfilConstraints(descriptor.AdditionalMetadataBundles);

                        if (!metadataConstraintsCheckResult.Success)
                        {
                            descriptor.MarkAsUnusable(PluginExclusionReason.PluginAdditionalMetadataConstraintsNotMet,
                                metadataConstraintsCheckResult.Result);

                            lstPluginDescriptorsForResult.Add(descriptor);

                            continue;
                        }

                        // Made it this far so mark as usable
                        descriptor.MarkAsUsable();
                        lstPluginDescriptorsForResult.Add(descriptor);
                    }

                    // Mark as initialised
                    _isInitialised = true;

                    return _pluginAsmInitialisationResultFactory
                        .CreateResultFromPlugins(lstPluginDescriptorsForResult.AsReadOnly());
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Something went wrong trying to load types in the plugin

                var loaderExceptions = ex.LoaderExceptions;

                var sb = new StringBuilder();
                sb.AppendLine("Failed to initialise plugin assembly, type loading errors:");

                foreach (var te in loaderExceptions)
                {
                    sb.AppendLine(te.Message);
                }

                throw new ApplicationException(sb.ToString(), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin assembly", ex);
            }
        }

        private readonly Dictionary<IPluginDescriptor, List<IPluginInstance>>
            _pluginInstances = new Dictionary<IPluginDescriptor, List<IPluginInstance>>();

        public IPluginInstance CreatePluginInstance(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Plugin descriptor must be supplied");

            IPluginInstance pluginInstance;

            try
            {
                if (!descriptor.IsUsable)
                {
                    throw new InvalidOperationException(string.Format("Plugin is marked as excluded: {0}",
                        descriptor.ExclusionReason.ToString()));
                }

                lock (_lock)
                {
                    lock (_pluginInstances)
                    {
                        List<IPluginInstance> instancesList;
                        if (_pluginInstances.TryGetValue(descriptor, out instancesList) == false)
                        {
                            instancesList = new List<IPluginInstance>();
                        }

                        pluginInstance = _pluginInstanceFactory.CreatePluginInstance(descriptor, this);
                        instancesList.Add(pluginInstance);

                        _pluginInstances[descriptor] = instancesList;
                    }
                }

                return pluginInstance;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin instance", ex);
            }
        }

        public void Unitialise()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        throw new InvalidOperationException("Plugin assembly isn't initialised");
                    }

                    // If there are any plugin instances they need bringing down
                    lock (_pluginInstances)
                    {
                        if (_pluginInstances.Count > 0)
                        {
                            foreach (IPluginInstance instance in _pluginInstances.Values.SelectMany(v => v))
                            {
                                if (instance.IsInitialised)
                                {
                                    instance.Unitialise(); 
                                }
                            }
                        }

                        _pluginInstances.Clear();
                    }

                    _pluginDescriptors = null;
                    _assemblyManager = null;
                    AppDomain.Unload(_appDomain);
                    _appDomain = null;
                    _isInitialised = false;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
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

        public string AssemblyFilePath
        {
            get { return _netAssemblyPath; }
        }
    }
}
