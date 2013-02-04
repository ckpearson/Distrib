using Distrib;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
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

                    })
                .Execute(job.Definition);
        }
    }
}
