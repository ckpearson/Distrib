using Distrib;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestLibrary.Dependance;

namespace TestLibrary
{
    [DistribProcessPlugin("Add integer process",
        "Adds two integers together",
        1.0,
        "Clint Pearson",
        "AddIntProc")]
    public sealed class AddIntegerProcess : CrossAppDomainObject, IPlugin, IProcess
    {
        void IPlugin.InitialisePlugin(IPluginInteractionLink interactionLink)
        {
            interactionLink.RegisterDependentAssembly(typeof(MathLib).Assembly.Location);
        }

        void IPlugin.UninitialisePlugin(IPluginInteractionLink interactionLink)
        {

        }

        void IProcess.InitProcess()
        {
        }

        void IProcess.UninitProcess()
        {
        }

        private IJobDefinition<IAddInput, IAddOutput> _def;
        IJobDefinition IProcess.JobDefinition
        {
            get
            {
                if (_def == null)
                {
                    _def = new ProcessJobDefinition<IAddInput, IAddOutput>("Add int job");
                    _def.ConfigInput(i => i.X).DefaultValue = 0;
                    _def.ConfigInput(i => i.Y).DefaultValue = 0;
                }

                return _def;
            }
        }

        void IProcess.ProcessJob(IJob job)
        {
            var input = new AddIntegerInput(job);
            var output = new AddIntegerOutput(job);

            output.Result = MathLib.Add(input.X, input.Y);
        }
    }

    public interface IAddInput
    {
        int X { get; }
        int Y { get; }
    }

    public interface IAddOutput
    {
        int Result { get; set; }
    }

    public sealed class AddIntegerInput : IAddInput
    {
        private readonly IJob _job;
        
        public AddIntegerInput(IJob job)
        {
            _job = job;
        }

        public int X
        {
            get { return _job.InputTracker.GetInput<int>(_job); }
        }

        public int Y
        {
            get { return _job.InputTracker.GetInput<int>(_job); }
        }
    }

    public sealed class AddIntegerOutput : IAddOutput
    {
        private readonly IJob _job;

        public AddIntegerOutput(IJob job)
        {
            _job = job;
        }

        public int Result
        {
            get
            {
                return _job.OutputTracker.GetOutput<int>(_job);
            }
            set
            {
                _job.OutputTracker.SetOutput<int>(_job, value);
            }
        }
    }


}
