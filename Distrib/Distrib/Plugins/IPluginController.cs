using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginController
    {
        void TakeRemoteBridge(RemoteAppDomainBridge bridge);
        object CreatePluginInstance(IPluginDescriptor descriptor, string pluginAssemblyPath);

        void InitController();
        void UninitController();

        void InitialiseInstance();
        void UnitialiseInstance();
    }
}
