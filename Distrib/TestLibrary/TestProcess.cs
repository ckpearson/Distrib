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
            var npjd = new NewTestProcessJobDefinition();
        }

        void IPlugin.UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
            
        }

        private WriteOnce<ProcessJobDefinition> _jobDefinition = new WriteOnce<ProcessJobDefinition>(null);
        ProcessJobDefinition IProcess.JobDefinition
        {
            get
            {
                if (!_jobDefinition.IsWritten)
                {
                    _jobDefinition.Value = new NewTestProcessJobDefinition();
                }

                return _jobDefinition.Value;
            }
        }
    }

    [Serializable()]
    public sealed class NewTestProcessJobDefinition : ProcessJobDefinition<INewTestProcessJobInputAccessor, INewTestProcessJobOutputAccessor>
    {
        public NewTestProcessJobDefinition()
            : base("New test process job")
        {
            
        }
    }

    public interface INewTestProcessJobInputAccessor
    {
        string SayHelloTo { get; set; }
    }

    public interface INewTestProcessJobOutputAccessor
    {
        string Message { get; set; }
    }
}
