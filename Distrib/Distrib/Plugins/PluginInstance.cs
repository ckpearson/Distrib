using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginInstance : MarshalByRefObject, IPluginInstance
    {
        private readonly IPluginDescriptor _pluginDescriptor;
        private readonly IPluginAssembly _pluginAssembly;

        private readonly WriteOnce<bool> _initialisedOnce = new WriteOnce<bool>(false);
        private AppDomain _appDomain;
        private RemoteAppDomainBridge _appDomainBridge;

        private readonly object _lock = new object();
        private bool _isInitialised = false;

        private WriteOnce<object> _underlyingInstance = new WriteOnce<object>(null);
        private WriteOnce<IPluginController> _pluginController = new WriteOnce<IPluginController>(null);

        private readonly string _instanceID;
        private readonly WriteOnce<DateTime> _instanceCreationStamp = new WriteOnce<DateTime>();

        private readonly IPluginInteractionLinkFactory _pluginInteractionLinkFactory;

        public PluginInstance(IPluginDescriptor descriptor, IPluginAssembly pluginAssembly, IPluginInteractionLinkFactory
            pluginInteractionLinkFactory)
        {
            _pluginDescriptor = descriptor;
            _pluginAssembly = pluginAssembly;

            _pluginInteractionLinkFactory = pluginInteractionLinkFactory;

            _instanceID = Guid.NewGuid().ToString();
        }

        public IPluginDescriptor PluginDescriptor
        {
            get { return _pluginDescriptor; }
        }

        public IPluginAssembly SpawningAssembly
        {
            get { return _pluginAssembly; }
        }

        public string InstanceID
        {
            get { return _instanceID; }
        }

        public DateTime InstanceCreationStamp
        {
            get
            {
                lock (_lock)
                {
                    if (!_underlyingInstance.IsWritten)
                    {
                        return DateTime.MinValue;
                    }
                    else
                    {
                        return _instanceCreationStamp.Value;
                    }
                }
            }
        }

        public T GetUnderlyingInstance<T>() where T : class
        {
            try
            {
                lock (_lock)
                {
                    if (!typeof(T).IsInterface)
                    {
                        throw new InvalidOperationException("Type must be an interface");
                    }

                    if (!typeof(T).IsAssignableFrom(_pluginDescriptor.Metadata.InterfaceType))
                    {
                        throw new InvalidOperationException("Type must be assignable from the original plugin interface type");
                    }

                    if (!IsInitialised)
                    {
                        throw new InvalidOperationException("Plugin instance must be initialised first");
                    }

                    if (!_underlyingInstance.IsWritten)
                    {
                        // Prepare for and create the controller
                        if ((_pluginDescriptor.Metadata.ControllerType.Assembly.Location !=
                            Assembly.GetExecutingAssembly().Location) &&
                            (_pluginDescriptor.Metadata.ControllerType.Assembly.Location !=
                                _pluginAssembly.AssemblyFilePath))
                        {
                            // The controller type is in a different assembly than the one executing currently and the
                            // assembly the plugin is in, this means the assembly needs loading into the domain

                            _appDomainBridge.LoadAssembly(_pluginDescriptor.Metadata.ControllerType.Assembly.Location);
                        }

                        // Now create the actual controller instance
                        _pluginController.Value = (IPluginController)_appDomainBridge.CreateInstance(
                            _pluginDescriptor.Metadata.ControllerType.FullName,
                            _pluginDescriptor.Metadata.ControllerType.Assembly.Location);

                        // Initialise the controller
                        _pluginController.Value.InitController();

                        // Send over remote domain bridge
                        _pluginController.Value.TakeRemoteBridge(_appDomainBridge);

                        // Get the controller to create the plugin instance
                        _underlyingInstance.Value = _pluginController.Value.CreatePluginInstance(_pluginDescriptor,
                            _pluginAssembly.AssemblyFilePath,
                            this,
                            _pluginInteractionLinkFactory);

                        // Set the creation stamp
                        _instanceCreationStamp.Value = DateTime.Now;

                        // Get the controller to initialise the plugin instance
                        _pluginController.Value.InitialiseInstance();
                    }

                    return (T)_underlyingInstance.Value;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get underlying instance", ex);
            }
        }

        public void Initialise()
        {
            try
            {
                lock (_lock)
                {
                    // Only support initialisation once
                    if (_initialisedOnce.IsWritten)
                    {
                        throw new InvalidOperationException("Initialisation only supported once per-instance");
                    }

                    if (IsInitialised)
                    {
                        throw new InvalidOperationException("Plugin instance already initialised");
                    }

                    // Set up the instance's AppDomain
                    _appDomain = AppDomain.CreateDomain(string.Format("{0}_{1}_{2}",
                        Guid.NewGuid(),
                        _pluginAssembly.AssemblyFilePath,
                        _pluginDescriptor.PluginTypeName));

                    _appDomain.AssemblyResolve += (s, e) =>
                        {
                            return AppDomain.CurrentDomain.GetAssemblies()
                                .DefaultIfEmpty(null)
                                .SingleOrDefault(asm => asm.FullName == e.Name);
                        };

                    // Create the remote bridge
                    _appDomainBridge = RemoteAppDomainBridge.FromAppDomain(_appDomain);

                    // Load the current assembly into the domain (to support plugin types)
                    _appDomainBridge.LoadAssembly(Assembly.GetExecutingAssembly().Location);

                    // Load the plugin assembly into the AppDomain
                    _appDomainBridge.LoadAssembly(_pluginAssembly.AssemblyFilePath);

                    _underlyingInstance = new WriteOnce<object>(null);

                    _isInitialised = true;
                    _initialisedOnce.Value = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin instance", ex);
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

        public void Unitialise()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        throw new InvalidOperationException("Plugin instance not initialised");
                    }

                    if (_pluginController.IsWritten)
                    {
                        // Get the controller to unitialise the instance
                        _pluginController.Value.UnitialiseInstance();

                        // Unitialise controller
                        _pluginController.Value.UninitController(); 
                    }

                    // Destroy bridge
                    _appDomainBridge = null;

                    // Unload domain
                    AppDomain.Unload(_appDomain);

                    // Cleanup
                    _appDomain = null;
                    _isInitialised = false;
                    _underlyingInstance = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to uninitialise plugin instance", ex);
            }
        }
    }
}
