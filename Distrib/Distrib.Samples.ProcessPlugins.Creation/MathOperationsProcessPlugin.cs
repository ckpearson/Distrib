#define JOB_CONFIG // Toggle to see the effects of having config against jobs

using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.PluginPowered;
using Distrib.Processes.Stock;
/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Samples.ProcessPlugins.Creation
{
    [DistribProcessPlugin(
        name: "Math operations",
        description: "Simple maths operations plugin",
        version: 1.0,
        author: "Clint Pearson",
        identifier: "{A5C4055F-89C7-4337-8F58-05BB297FAF4D}")]
    public sealed class MathOperationsProcessPlugin :
        CrossAppDomainObject,
        IPlugin,
        IProcess
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

        private IReadOnlyList<IJobDefinition> _definitions = null;

        IReadOnlyList<IJobDefinition> IProcess.JobDefinitions
        {
            get
            {
                if (_definitions == null)
                {
                    _definitions = typeof(MathOpsDefinitions)
                        .GetProperties()
                        .Where(p => p.PropertyType.Equals(typeof(IJobDefinition)) &&
                            p.CanRead)
                        .Select(p => (IJobDefinition)p.GetValue(null))
                        .ToList()
                        .AsReadOnly();
                }

                return _definitions;
            }
        }

        void IProcess.ProcessJob(IJob job)
        {
            var input = JobDataHelper<IStockInput<int,int>>
                .New(job.Definition).ForInputGet(job);

            var output = JobDataHelper<IStockOutput<int>>
                .New(job.Definition).ForOutputSet(job);

            JobExecutionHelper.New()
                .AddHandler(() => MathOpsDefinitions.Add,
                    () =>
                    {
                        output.Set(o => o.Output,
                            input.Get(i => i.FirstInput) + input.Get(i => i.SecondInput));
                    })
                .AddHandler(() => MathOpsDefinitions.Subtract,
                    () =>
                    {
                        output.Set(o => o.Output,
                            input.Get(i => i.FirstInput) - input.Get(i => i.SecondInput));
                    })
                .AddHandler(() => MathOpsDefinitions.Multiply,
                    () =>
                    {
                        output.Set(o => o.Output,
                            input.Get(i => i.FirstInput) * input.Get(i => i.SecondInput));
                    })
                .AddHandler(() => MathOpsDefinitions.Divide,
                    () =>
                    {
                        output.Set(o => o.Output,
                            input.Get(i => i.FirstInput) / input.Get(i => i.SecondInput));
                    })
                .Execute(job.Definition);
        }
    }

    public static class MathOpsDefinitions
    {
        private static ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>> _addDef;
        private static ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>> _subDef;
        private static ProcessJobDefinition<IStockInput<int,int>, IStockOutput<int>> _mulDef;
        private static ProcessJobDefinition<IStockInput<int,int>, IStockOutput<int>> _divDef;

        public static IJobDefinition Add
        {
            get
            {
                if (_addDef == null)
                {
                    _addDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>(
                        "Add",
                        "Adds two integers");

#if  JOB_CONFIG
                    _addDef.ConfigInput(i => i.FirstInput).DisplayName = "x";
                    _addDef.ConfigInput(i => i.SecondInput).DisplayName = "y";
#endif
                }

                return _addDef;
            }
        }

        public static IJobDefinition Subtract
        {
            get
            {
                if (_subDef == null)
                {
                    _subDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>(
                        "Subtract",
                        "Subtracts two integers");

#if JOB_CONFIG
                    _subDef.ConfigInput(i => i.FirstInput).DisplayName = "x";
                    _subDef.ConfigInput(i => i.SecondInput).DisplayName = "y";
#endif
                }


                return _subDef;
            }
        }

        public static IJobDefinition Multiply
        {
            get
            {
                if (_mulDef == null)
                {
                    _mulDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>(
                        "Multiply",
                        "Multiplies two integers");

#if  JOB_CONFIG
                    _mulDef.ConfigInput(i => i.FirstInput).DisplayName = "x";
                    _mulDef.ConfigInput(i => i.SecondInput).DisplayName = "y";
#endif
                }

                return _mulDef;
            }
        }

        public static IJobDefinition Divide
        {
            get
            {
                if (_divDef == null)
                {
                    _divDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>(
                        "Divide",
                        "Divides two integers");

#if JOB_CONFIG
                    _divDef.ConfigInput(i => i.FirstInput).DisplayName = "x";
                    _divDef.ConfigInput(i => i.SecondInput).DisplayName = "y";
#endif
                    
                }

                return _divDef;
            }
        }
    }
}
