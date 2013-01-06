using Distrib.IOC;
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
    public sealed class ProcessHost : MarshalByRefObject, IProcessHost
    {
        public ProcessHost([IOC(false)] IPluginDescriptor descriptor, IPluginAssemblyFactory fact)
        {

        }

        public void Initialise()
        {
            
        }

        public void Unitialise()
        {
            
        }
    }
}
