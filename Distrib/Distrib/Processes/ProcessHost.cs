using Distrib.Plugins;
using Distrib.Separation;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHost : IProcessHost
    {
        private readonly WriteOnce<AppDomain> _appDomain = new WriteOnce<AppDomain>(null);
        
        private readonly IPluginInstance _pluginInstance = null;

        private readonly WriteOnce<bool> _initOnce = new WriteOnce<bool>(false);

        private readonly object _lock = new object();

        private bool _isInitialised = false;

        public ProcessHost(IPluginInstance pluginInstance, IRemoteDomainBridgeFactory bridgeFactory)
        {
            if (pluginInstance == null) throw new ArgumentNullException();
            if (!pluginInstance.PluginDescriptor.OfPluginInterface<IProcess>())
                throw new InvalidOperationException("Given plugin instance isn't a process");

            _pluginInstance = pluginInstance;
        }

        public void Initialise()
        {
            try
            {
                if (_initOnce.Value)
                {
                    throw new InvalidOperationException("Process host can only be initialised once");
                }

                _appDomain.Value = AppDomain.CreateDomain(string.Format("procHostDomain_{0}{1},{2}",
                    Guid.NewGuid(),
                    _pluginInstance.InstanceID,
                    _pluginInstance.PluginDescriptor.PluginTypeName));

                _isInitialised = true;
                _initOnce.Value = true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise", ex);
            }
        }

        public void Unitialise()
        {
            throw new NotImplementedException();
        }
    }
}
