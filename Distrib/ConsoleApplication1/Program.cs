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

using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Stock;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication1
{
    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();
            p.InputHelperTest();
        }

        private void InputHelperTest()
        {
            IIOC nboot = new Distrib.IOC.Ninject.NinjectBootstrapper();
            nboot.Start();

            var processHostFactory =
                nboot.Get<IProcessHostFactory>();

            var procHost =
                processHostFactory.CreateHostFromTypeSeparated(typeof(SomeProcess));

            procHost.Initialise();

            var result = procHost.ProcessJob(procHost.JobDefinitions[0],
                JobDataHelper<IStockInput<string>>
                .New(procHost.JobDefinitions[0])
                .ForInputSet()
                .Set(i => i.Input, "Clint")
                .ToValueFields());

            var message = JobDataHelper<IStockOutput<string>>
                .New(procHost.JobDefinitions[0])
                .ForOutputGet(result)
                .Get(o => o.Output);

            procHost.Unitialise();
            procHost = null;
        }

        private void RunNewIOCTest()
        {
            var kernel = new StandardKernel(new INinjectModule[] { });
            IIOC nboot = new Distrib.IOC.Ninject.NinjectBootstrapper(kernel);
            nboot.Start();
        }

        private void DashedLine()
        {
            Console.WriteLine(new string(Enumerable.Repeat('*', Console.BufferWidth - 5).ToArray()));
        }
    }

    public sealed class SomeProcess : IProcess
    {

        public void InitProcess()
        {
        }

        public void UninitProcess()
        {
        }

        private IJobDefinition<IStockInput<string>, IStockOutput<string>> _sayHelloJob;
        private IReadOnlyList<IJobDefinition> _jobs = null;
        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get
            {
                if (_jobs == null)
                {
                    if (_sayHelloJob == null)
                    {
                        _sayHelloJob = new ProcessJobDefinition<IStockInput<string>, IStockOutput<string>>(
                            "Say Hello", "Says hello to someone");
                    }

                    _jobs = new List<IJobDefinition>()
                    {
                        _sayHelloJob,
                    }.AsReadOnly();
                }

                return _jobs;
            }
        }

        public void ProcessJob(IJob job)
        {
            JobExecutionHelper.New()
                .AddHandler(() => _sayHelloJob, () =>
                    {
                        var input = JobDataHelper<IStockInput<string>>
                            .New(job.Definition)
                            .ForInputGet(job);

                        var output = JobDataHelper<IStockOutput<string>>
                            .New(job.Definition)
                            .ForOutputSet(job);

                        output.Set(o => o.Output,
                            string.Format("Hello, {0}",
                                input.Get(i => i.Input)));
                    })
                .Execute(job.Definition);
        }
    }
}