using Distrib.Plugins_old.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old.Controllers
{
    /// <summary>
    /// Interface for a plugin controller
    /// </summary>
    public interface IDistribPluginController
    {
        void StoreAppDomainBridge(RemoteAppDomainBridge bridge);
        object CreatePluginInstance(PluginDetails pluginDetails, string parentAssemblyPath);

        void InitController();
        void UnitController();

        void InitialiseInstance();
        void UnitialiseInstance();
    }
}
