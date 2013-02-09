using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessHostTypeService
    {
        bool IsTypePowered(IProcessHost host);
        bool IsPluginPowered(IProcessHost host);

        ITypePoweredProcessHost GetTypePoweredInterface(IProcessHost host);
        IPluginPoweredProcessHost GetPluginPoweredInterface(IProcessHost host);

        SystemPowerType GetPowerType(IProcessHost host);
    }
}
