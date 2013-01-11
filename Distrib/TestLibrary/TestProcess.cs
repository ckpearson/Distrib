using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    [DistribProcessPlugin("New test process",
        "new process for the new plugin system",
        1.0,
        "Clint Pearson",
        "identifier")]
    public class NewTestProcess : MarshalByRefObject, IPlugin, IProcess
    {
        void IPlugin.InitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }

        void IPlugin.UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
            
        }

        void IProcess.InitProcess()
        {
            // The process has been loaded into a host and is to initialise
        }

        void IProcess.UninitProcess()
        {
            // The process is about to be unloaded by a host and is to unitialise
        }

        private ProcessJobDefinition<IInput, IOutput> _def;
        IJobDefinition IProcess.JobDefinition
        {
            get
            {
                if (_def == null)
                {
                    _def = new ProcessJobDefinition<IInput, IOutput>();

                }

                return _def;
            }
        }
    }

    public interface IInput
    {
        string SayHelloTo { get; set; }
        DateTime Something { get; set; }
    }

    public interface IOutput
    {
        string Message { get; set; }
    }
}
