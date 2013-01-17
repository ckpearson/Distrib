using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class StandardPluginController : CrossAppDomainObject, IPluginController
    {
        private WriteOnce<RemoteAppDomainBridge> _appDomainBridge =
            new WriteOnce<RemoteAppDomainBridge>(null);

        private WriteOnce<IPluginDescriptor> _pluginDescriptor =
            new WriteOnce<IPluginDescriptor>(null);

        private WriteOnce<IPlugin> _pluginInstance =
            new WriteOnce<IPlugin>(null);

        private WriteOnce<IPluginInteractionLink> _pluginInteractionLink = new WriteOnce<IPluginInteractionLink>(null);

        private readonly object _lock = new object();

        public StandardPluginController()
        {
        }

        public void TakeRemoteBridge(RemoteAppDomainBridge bridge)
        {
            lock (_lock)
            {
                if (!_appDomainBridge.IsWritten)
                {
                    _appDomainBridge.Value = bridge;
                }
                else
                {
                    throw new InvalidOperationException("Bridge already supplied");
                }
            }
        }

        public object CreatePluginInstance(IPluginDescriptor descriptor, string pluginAssemblyPath,
            IPluginInstance pluginManagedInstance,
            IPluginInteractionLinkFactory pluginInteractionLinkFactory)
        {
            if (descriptor == null) throw new ArgumentNullException("Plugin descriptor must be supplied");
            if (string.IsNullOrEmpty(pluginAssemblyPath)) throw new ArgumentNullException("Plugin assembly path must be supplied");

            try
            {
                lock (_lock)
                {
                    if (!_pluginInstance.IsWritten)
                    {
                        _pluginDescriptor.Value = descriptor;

                        _pluginInstance.Value = (IPlugin)_appDomainBridge.Value.CreateInstance(descriptor.PluginTypeName,
                            pluginAssemblyPath);

                        _pluginInteractionLink.Value = pluginInteractionLinkFactory.CreateInteractionLink(
                            descriptor,
                            _pluginInstance.Value,
                            this,
                            pluginManagedInstance);

                        return _pluginInstance.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Controller already created plugin instance");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin instance", ex);
            }
        }

        public void InitController()
        {
            // Controller initialisation
        }

        public void UninitController()
        {
            // Controller uninitialisation (not to unitialise instance here)
        }

        public void InitialiseInstance()
        {
            if (_pluginInstance.IsWritten)
            {
                _pluginInstance.Value.InitialisePlugin(_pluginInteractionLink.Value);
            }
            else
            {
                throw new InvalidOperationException("Controller doesn't hold an instance yet");
            }
        }

        public void UnitialiseInstance()
        {
            if (_pluginInstance.IsWritten)
            {
                _pluginInstance.Value.UninitialisePlugin(_pluginInteractionLink.Value);
            }
            else
            {
                throw new InvalidProgramException("Controller doesn't hold an instance yet");
            }
        }
    }
}
