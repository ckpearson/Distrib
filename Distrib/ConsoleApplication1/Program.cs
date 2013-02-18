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

using Distrib.Communication;
using Distrib.IOC;
using Distrib.IOC.Ninject;
using Distrib.Nodes.Process;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Processes.Stock;
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
    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();

            p.DoProcessNodeTest();


            Console.ReadLine();
        }

        private async void DoProcessNodeTest()
        {
            var nboot = new NinjectBootstrapper();
            nboot.Start();

            

            var mi = this.GetType().GetMethod("DoSomething");
            var actArg = mi.GetParameters()[0];

            var vlist = new List<object>();

            var d = BuildDynamicAction(actArg, out vlist);

            mi.Invoke(this, new object[] { d });

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

        public void DoSomething(Action<string> act)
        {
            act("Hello");
        }
    }

    public sealed class HostCommsWrapper
    {
        private readonly IProcessHost _host;
        private IIncomingCommsLink<IProcessHost> _hostComms;

        public HostCommsWrapper(IProcessHost host)
        {
            _host = host;
            _hostComms = new TcpIncomingCommsLink<IProcessHost>(IPAddress.Any,
                8080,
                new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
                new DirectInvocationCommsMessageProcessor());
            _hostComms.StartListening(_host);
        }
    }

    public sealed class HostCommsProxy
        : OutgoingCommsProxyBase, IProcessHost
    {
        public HostCommsProxy(IOutgoingCommsLink<IProcessHost> outgoingLink)
            : base(outgoingLink)
        {

        }

        public void Initialise()
        {
            Link.InvokeMethod(null);
        }

        public void Unitialise()
        {
            Link.InvokeMethod(null);
        }

        public bool IsInitialised
        {
            get { return (bool)Link.GetProperty(); }
        }

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get { return (IReadOnlyList<IJobDefinition>)Link.GetProperty(); }
        }

        public void QueueJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues, Action<IReadOnlyList<IProcessJobValueField>, object> onCompletion, object data, Action<Exception> onException)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IProcessJobValueField> QueueJobAndWait(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputValues)
        {
            throw new NotImplementedException();
        }

        public int QueuedJobs
        {
            get { throw new NotImplementedException(); }
        }

        public bool JobExecuting
        {
            get { throw new NotImplementedException(); }
        }

        public IJobDefinition ExecutingJob
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime InstanceCreationStamp
        {
            get { throw new NotImplementedException(); }
        }

        public string InstanceID
        {
            get { throw new NotImplementedException(); }
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


    public interface IAbcComms
    {
        void DoSomethingOverComms();
        string SayHello(string name);

        int Number { get; set; }
    }

    public interface IAbc
    {
        void DoSomething();
    }

    public sealed class Abc : IAbc, IAbcComms, IDisposable
    {
        private IIncomingCommsLink<IAbcComms> _incomingComms;

        public Abc(IIncomingCommsLink<IAbcComms> incomingComms)
        {
            _incomingComms = incomingComms;
            _incomingComms.StartListening(this as IAbcComms);
        }

        public void DoSomething()
        {

        }

        public void Dispose()
        {
            if (_incomingComms != null)
            {
                if (_incomingComms.IsListening)
                {
                    _incomingComms.StopListening();
                }
            }
        }

        public void DoSomethingOverComms()
        {
        }

        public string SayHello(string name)
        {
            return string.Format("Hello, {0}", name);
        }

        public int Number
        {
            get;
            set;
        }
    }

    public sealed class AbcOutgoingProxy : OutgoingCommsProxyBase, IAbcComms
    {
        public AbcOutgoingProxy(IOutgoingCommsLink<IAbcComms> link)
            : base(link)
        {

        }

        public void DoSomethingOverComms()
        {
            this.Link.InvokeMethod(null);
        }


        public string SayHello(string name)
        {
            return (string)this.Link.InvokeMethod(new[] { name });
        }
        public int Number
        {
            get { return (int)this.Link.GetProperty(); }
            set
            {
                this.Link.SetProperty(value);
            }
        }
    }
}