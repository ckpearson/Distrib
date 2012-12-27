using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHostFactory : IProcessHostFactory
    {
        public IProcessHost CreateHostForProcessPlugin(Plugins.IPluginInstance pluginInstance)
        {
            return new ProcessHost();
        }
    }
}
