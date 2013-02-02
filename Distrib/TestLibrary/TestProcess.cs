/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
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
    //[DistribProcessPlugin("CrossDomainExcludedPlugin",
    //    "Plugin which should be excluded because it isn't a cross domain derivative",
    //    1.0,
    //    "Clint Pearson",
    //    "CROSSDOMAINDERIV")]
    //public sealed class CrossDomainExcludedProc : IPlugin, IProcess
    //{

    //    public void InitialisePlugin(IPluginInteractionLink interactionLink)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UninitialisePlugin(IPluginInteractionLink interactionLink)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void InitProcess()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UninitProcess()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IReadOnlyList<IJobDefinition> JobDefinitions
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public void ProcessJob(IJob job)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //[DistribProcessPlugin("CrossDomainExcludedPluginA",
    //"Plugin which should be excluded because it isn't a cross domain derivative",
    //1.0,
    //"Clint Pearson",
    //"CROSSDOMAINDERIVA")]
    //public sealed class CrossDomainExcludedProcA : IPlugin, IProcess
    //{

    //    public void InitialisePlugin(IPluginInteractionLink interactionLink)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UninitialisePlugin(IPluginInteractionLink interactionLink)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void InitProcess()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UninitProcess()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IReadOnlyList<IJobDefinition> JobDefinitions
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public void ProcessJob(IJob job)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [DistribProcessPlugin("IntOperationsProcess", "Performs mathematical operations on integers", 1.0, "Clint Pearson", "{FE010226-FDF3-4823-A620-4EEB69A3FDBE}")]
    public sealed class IntOperationsProc : CrossAppDomainObject, IPlugin, IProcess
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

        private WriteOnce<IReadOnlyList<IJobDefinition>> _definitions = new WriteOnce<IReadOnlyList<IJobDefinition>>(null);

        IReadOnlyList<IJobDefinition> IProcess.JobDefinitions
        {
            get
            {
                lock (_definitions)
                {
                    if (!_definitions.IsWritten)
                    {
                        _definitions.Value = new List<IJobDefinition>()
                        {
                            IntOperationsProcJobs.AddJobDef,
                            IntOperationsProcJobs.SubJobDef,
                        }.AsReadOnly();
                    }

                    return _definitions.Value;
                }
            }
        }

        void IProcess.ProcessJob(IJob job)
        {
            JobExecutionHelper.New()
                .AddHandler(() => IntOperationsProcJobs.AddJobDef, () =>
                    {
                        // Perform add int job
                        var inp = new AddInput(job);
                        var outp = new StockOutput<int>(job);

                        outp.Output = inp.X + inp.Y;
                    })
                .AddHandler(() => IntOperationsProcJobs.SubJobDef, () =>
                    {
                        var inp = new StockInput<int, int>(job);
                        var outp = new StockOutput<int>(job);

                        outp.Output = inp.FirstInput - inp.SecondInput;
                    })
                .Execute(job.Definition);
        }
    }

    public static class IntOperationsProcJobs
    {
        private static IJobDefinition<IAddInput, IStockOutput<int>> _addJobDef;
        public static IJobDefinition AddJobDef
        {
            get
            {
                if (_addJobDef == null)
                {
                    _addJobDef = new ProcessJobDefinition<IAddInput, IStockOutput<int>>("Add",
                        "Adds two integers together");

                    _addJobDef.ConfigOutput(o => o.Output).DisplayName = "Result (X + Y)";
                }

                return _addJobDef;
            }
        }

        private static IJobDefinition<IStockInput<int, int>, IStockOutput<int>> _subJobDef;
        public static IJobDefinition SubJobDef
        {
            get
            {
                if (_subJobDef == null)
                {
                    _subJobDef = new ProcessJobDefinition<IStockInput<int, int>, IStockOutput<int>>("Subtract",
                        "Subtracts two integers");

                    _subJobDef.ConfigInput(i => i.FirstInput).DisplayName = "X";
                    _subJobDef.ConfigInput(i => i.SecondInput).DisplayName = "Y";

                    _subJobDef.ConfigOutput(o => o.Output).DisplayName = "Result (X - Y)"; 
                }

                return _subJobDef;
            }
        }
    }


    public interface IAddInput
    {
        int X { get; }
        int Y { get; }
    }

    public sealed class AddInput : IAddInput
    {
        private readonly IJob _job;

        public AddInput(IJob job)
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
}
