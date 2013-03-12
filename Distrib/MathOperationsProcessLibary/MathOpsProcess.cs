using Distrib;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.PluginPowered;
using Distrib.Processes.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathOperationsProcessLibary
{
    [DistribProcessPlugin(
        "Mathematical Operations Process",
        "Performs mathematical operations",
        1.0,
        "Clint Pearson",
        "{FF36C822-A8A0-4A8F-A313-0842E5D4C5EE}")]
    public sealed class MathOpsProcess : CrossAppDomainObject, IPlugin, IProcess
    {
        void IPlugin.InitialisePlugin(IPluginInteractionLink interactionLink)
        {
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

        IReadOnlyList<IJobDefinition> IProcess.JobDefinitions
        {
            get
            {
                return MathOpsProcess_JobDefinitions.Definitions;
            }
        }

        void IProcess.ProcessJob(IJob job)
        {
            JobExecutionHelper.New()
                .AddHandler(() => MathOpsProcess_JobDefinitions.AddIntDef,
                    () =>
                    {
                        var input = JobDataHelper<IStockInput<int, int>>
                            .New(job.Definition)
                            .ForInputGet(job);

                        var output = JobDataHelper<IStockOutput<int>>
                            .New(job.Definition)
                            .ForOutputSet(job);

                        output.Set(o => o.Output,
                            input.Get(i => i.FirstInput) + input.Get(i => i.SecondInput));
                    })
                .Execute(job.Definition);
        }
    }

    [Distrib.Processes.TypePowered.ProcessMetadata("Dummy", "Dummy type process", 1.0, "Clint")]
    public sealed class DummyTypeProcess : CrossAppDomainObject, IProcess
    {
        public void InitProcess()
        {
        }

        public void UninitProcess()
        {
        }

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get { return null; }
        }

        public void ProcessJob(IJob job)
        {
        }
    }

    [Distrib.Processes.TypePowered.ProcessMetadata("Dummy 2", "Dummy type process", 1.0, "Clint")]
    public sealed class DummyTypeProcess2 : CrossAppDomainObject, IProcess
    {
        public void InitProcess()
        {
        }

        public void UninitProcess()
        {
        }

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get { return null; }
        }

        public void ProcessJob(IJob job)
        {
        }
    }

}
