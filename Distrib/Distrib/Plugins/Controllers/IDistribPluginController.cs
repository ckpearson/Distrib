using Distrib.Plugins.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Controllers
{
    /// <summary>
    /// Interface for a plugin controller
    /// </summary>
    public interface IDistribPluginController
    {
        void StoreAppDomainBridge(RemoteAppDomainBridge bridge);
        object CreatePluginInstance(DistribPluginDetails pluginDetails, string parentAssemblyPath);
    }
}
