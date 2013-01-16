using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLibrary
{
    [DistribProcessPlugin("invalid process",
        "An invalid process",
        1.0,
        "Clint Pearson",
        "invalid")]
    public class InvalidProcess : IPlugin
    {

        public void InitialisePlugin(IPluginInteractionLink interactionLink)
        {
            throw new NotImplementedException();
        }

        public void UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
            throw new NotImplementedException();
        }
    }

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
                    _def = new ProcessJobDefinition<IInput, IOutput>("New test process job");

                    _def.ConfigInput(i => i.SayHelloTo).DefaultValue = "Bob";
                }

                return _def;
            }
        }

        public void ProcessJob(IJob job)
        {
            var input = new NewTestProcessInput(job);

            Thread.Sleep(3000);

            var output = new NewTestProcessOutput(job);

            output.Message = string.Format("Hello, {0}!", input.SayHelloTo);
        }
    }

    [Serializable()]
    public sealed class NewTestProcessInput : IInput
    {
        private readonly IJob _job;

        public NewTestProcessInput(IJob job)
        {
            _job = job;
        }

        public string SayHelloTo
        {
            get
            {
                return _job.InputTracker.GetInput<string>(_job);
            }
        }
    }

    [Serializable()]
    public sealed class NewTestProcessOutput : IOutput
    {
        private readonly IJob _job;

        public NewTestProcessOutput(IJob job)
        {
            _job = job;
        }

        public string Message
        {
            get
            {
                return _job.OutputTracker.GetOutput<string>(_job);
            }

            set
            {
                _job.OutputTracker.SetOutput<string>(_job, value);
            }
        }
    }

    public interface IInput
    {
        string SayHelloTo { get; }
    }

    public interface IOutput
    {
        string Message { get; set; }
    }
}
