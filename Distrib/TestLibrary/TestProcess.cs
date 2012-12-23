using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [DistribProcessPlugin("New test process", "new process for the new plugin system", 1.0, "Clint Pearson", "identifier")]
    public class NewTestProcess : MarshalByRefObject, IPlugin, IDistribProcess
    {
        public string SayHello()
        {
            return "Hello thar";
        }

        public void InitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }

        public void UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }
    }
}
