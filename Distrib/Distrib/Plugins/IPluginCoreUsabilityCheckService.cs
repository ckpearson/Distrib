using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginCoreUsabilityCheckService
    {
        Res<PluginExclusionReason, object> CheckCoreUsability(IPluginDescriptor descriptor,
            IPluginAssemblyManager assemblyManager);
    }
}
