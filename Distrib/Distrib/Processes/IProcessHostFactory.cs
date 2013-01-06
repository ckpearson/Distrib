using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessHostFactory
    {
        IProcessHost CreateHostFromPlugin(IPluginDescriptor descriptor);
        IProcessHost CreateHostFromPluginSeparated(IPluginDescriptor descriptor);
    }
}
