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
        private WriteOnce<IProcessJobDefinition> _jobDefinition = new WriteOnce<IProcessJobDefinition>(null);

        void IPlugin.InitialisePlugin(IPluginInteractionLink interactionLink)
        {
            
        }

        void IPlugin.UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }

        void IProcess.PerformJob(IProcessJob job)
        {
            
        }

        IProcessJobDefinition IProcess.JobDefinition
        {
            get
            {
                lock (_jobDefinition)
                {
                    if (!_jobDefinition.IsWritten)
                    {
                        _jobDefinition.Value = new NewTestProcessJobDefinition();
                    }

                    return _jobDefinition.Value;
                }
            }
        }
    }

    public sealed class NewTestProcessJobDefinition : ProcessJobDefinition<INewTestProcessJobInputFront>
    {
        public NewTestProcessJobDefinition()
            : base("NewTestProcessJob")
        {
            
        }
    }

    public interface INewTestProcessJobInputFront
    {
        string SayHelloTo { get; }
    }

    public sealed class NewTestProcessJobInput : ProcessJobInput
    {

    }
}
