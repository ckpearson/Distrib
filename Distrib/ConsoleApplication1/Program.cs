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
using Distrib.Communication;
using Distrib.Data.Transport;
using Distrib.IOC;
using Distrib.IOC.Ninject;
using Distrib.Nodes.Process;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Stock;
using Distrib.Processes.TypePowered;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApplication1
{

    public sealed class Person
    {
        [DataTransportPoint(DataTransportPointDirection.Both, new []
            {
                "fname",
                "firstname",
                "first name",
                "forename"
            })]
        public string FName { get; set; }
        public string LName { get; set; }
    }

    public sealed class Student
    {
        [DataTransportPoint(DataTransportPointDirection.Both, new[]
            {
                "firstname",
                "first name",
                "forename",
            })]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [DataTransportPoint(DataTransportPointDirection.Both, new[]
            {
                "lastname",
                "last name",
                "surname",
                "family name",
            })]
        public string LastName { get; set; }
    }

    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();

            p.RunDataTransportTest();

            //p.NaiveDistribTest();

            Console.ReadLine();
        }

        private void RunDataTransportTest()
        {
            var nboot = new NinjectBootstrapper();
            nboot.Start();

            var svc = nboot.Get<IDataTransportService>();

            var student = new Student()
            {
                FirstName = "Clint",
                LastName = "Pearson",
            };

            var person = svc.MapLTR(student, new Person());

            var s = "";
        }

        public Delegate BuildDynamicAction(ParameterInfo actionParameter, out List<object> valuesList)
        {
            if (actionParameter.ParameterType.GenericTypeArguments == null ||
                actionParameter.ParameterType.GenericTypeArguments.Length == 0)
            {
                throw new ArgumentException();
            }

            valuesList = new List<object>();

            var args = actionParameter.ParameterType.GenericTypeArguments;

            var lt = Expression.Label();
            var valVar = Expression.Variable(typeof(List<object>), "vals");
            var para = args.Select(a => Expression.Parameter(a)).ToArray();

            var setters = new List<Expression>();

            var addvalMeth = valuesList.GetType().GetMethod("Add");

            foreach (var p in para)
            {
                setters.Add(Expression.Call(valVar,
                    addvalMeth, p));
            }

            var block = Expression.Block(
                variables: new ParameterExpression[]
                {
                    valVar,
                },
                expressions: Enumerable.Concat(
                    para,
                    new Expression[]
                    {
                        Expression.Assign(valVar, Expression.Constant(valuesList)),
                        Expression.Call(valVar, valuesList.GetType().GetMethod("Clear")),
                    }.Concat(setters)
                    .Concat(
                        new Expression[]
                        {
                            Expression.Return(lt),
                            Expression.Label(lt),
                        })));

            return Expression.Lambda(block, para).Compile();
        }
    }

    [ProcessMetadata("Some process",
        "some simple process",
        1.0,
        "Clint Pearson")]
    public sealed class SomeProcess : IProcess
    {
        public void InitProcess()
        {
        }

        public void UninitProcess()
        {
        }

        private ProcessJobDefinition<IStockInput<string>, IStockOutput<string>>
            _sayHelloJob;

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get
            {
                if (_sayHelloJob == null)
                {
                    _sayHelloJob = new ProcessJobDefinition<IStockInput<string>, IStockOutput<string>>("Say Hello",
                        "Says hello to someone");
                    _sayHelloJob.ConfigInput(i => i.Input)
                        .DisplayName = "Person Name";
                }

                return new[]
                {
                    _sayHelloJob,
                }.ToList().AsReadOnly();
            }
        }

        public void ProcessJob(IJob job)
        {
            JobExecutionHelper.New()
                .AddHandler(() => _sayHelloJob,
                    () =>
                    {
                        JobDataHelper<IStockOutput<string>>
                            .New(job.Definition)
                            .ForOutputSet(job)
                            .Set(o => o.Output,
                                string.Format("Hello, {0}", JobDataHelper<IStockInput<string>>
                                .New(job.Definition)
                                .ForInputGet(job)
                                .Get(i => i.Input)));
                    })
                .Execute(job.Definition);
        }
    }
}