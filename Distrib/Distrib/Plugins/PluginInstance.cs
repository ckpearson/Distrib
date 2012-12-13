using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginInstance : IPluginInstance
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

        public PluginInstance(IPluginDescriptor descriptor, IPluginAssembly pluginAssembly)
        {
            _pluginDescriptor = descriptor;
            _pluginAssembly = pluginAssembly;
        }

        public string InstanceID
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime InstanceCreationStamp
        {
            get { throw new NotImplementedException(); }
        }

        public T GetUnderlyingInstance<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public bool IsInitialised
        {
            get { throw new NotImplementedException(); }
        }

        public void Unitialise()
        {
            throw new NotImplementedException();
        }
    }
}
