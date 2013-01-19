using Distrib;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Plugin;
using Distrib.Processes.Stock;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestLibrary
{
    [DistribProcessPlugin("AddIntProcess", "Adds two integers together", 1.0, "Clint Pearson", "{CBE47BC5-9350-4CB1-938B-69A0E07037E5}")]
    public sealed class AddIntProc : CrossAppDomainObject, IPlugin, IProcess
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

        private IJobDefinition<IStockInput<int, int>, IStockOutput<int>> _def;
        IJobDefinition IProcess.JobDefinition
        {
            get
            {
                if (_def == null)
                {
                    _def = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>("AddInt");
                    _def.ConfigInput(i => i.FirstInput).DisplayName = "x";
                    _def.ConfigInput(i => i.SecondInput).DisplayName = "y";

                    _def.ConfigOutput(i => i.Output).DisplayName = "Result";
                }

                return _def;
            }
        }

        void IProcess.ProcessJob(IJob job)
        {
            var input = new StockInput<int, int>(job);
            var output = new StockOutput<int>(job);

            output.Output = input.FirstInput + input.SecondInput;
        }
    }
}
