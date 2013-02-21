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
    public interface IHostComms
    {
        void Initialise();
        IReadOnlyList<IJobDefinition> GetJobDefinitions();
        IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs);
        bool SupportsDefinition(IJobDefinition definition);
        bool IsProcessingJob();
        int QueuedJobCount();
    }

    public sealed class HostExposer
        : IHostComms
    {
        private readonly IProcessHost _host;
        private readonly IIncomingCommsLink<IHostComms> _link;
        private readonly HostTrackerProxy _tracker;

        public HostExposer(IProcessHost host, IIncomingCommsLink<IHostComms> incomingLink, HostTrackerProxy tracker)
        {
            _host = host;
            _tracker = tracker;
            _link = incomingLink;
            _link.StartListening(this);
        }

        public void Initialise()
        {
            _host.Initialise();
            _tracker.HostOnline(_link.GetEndpointDetails());
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            var jds = _host.JobDefinitions;
            var lst = new List<IJobDefinition>();

            foreach (var jd in jds)
            {
                lst.Add(jd.ToFlattened());
            }

            return lst.AsReadOnly();
        }


        public IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs)
        {
            return _host.QueueJobAndWait(definition, inputs);
        }


        public bool SupportsDefinition(IJobDefinition definition)
        {
            return GetJobDefinitions().Any(d => d.Match(definition));
        }

        public bool IsProcessingJob()
        {
            return _host.JobExecuting;
        }

        public int QueuedJobCount()
        {
            return _host.QueuedJobs;
        }
    }

    public sealed class HostExposerProxy
        : OutgoingCommsProxyBase,
        IHostComms
    {
        public HostExposerProxy(IOutgoingCommsLink<IHostComms> link)
            : base(link)
        {

        }

        public void Initialise()
        {
            Link.InvokeMethod(null);
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            return Link.InvokeMethod(null) as IReadOnlyList<IJobDefinition>;
        }


        public IReadOnlyList<IProcessJobValueField> ProcessJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs)
        {
            return Link.InvokeMethod(new object[] { definition.ToFlattened(), inputs }) as IReadOnlyList<IProcessJobValueField>;
        }


        public bool SupportsDefinition(IJobDefinition definition)
        {
            return (bool)Link.InvokeMethod(new object[] { definition });
        }

        public bool IsProcessingJob()
        {
            return (bool)Link.InvokeMethod(null);
        }

        public int QueuedJobCount()
        {
            return (int)Link.InvokeMethod(null);
        }
    }

    public interface ITrackerComms
    {
        void HostOnline(object endpoint);
        void HostOffline(object endpoint);

        IReadOnlyList<IJobDefinition> GetJobDefinitions();

        IReadOnlyList<IProcessJobValueField> SubmitJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs);
    }

    public sealed class HostTrackerProxy
        : OutgoingCommsProxyBase,
        ITrackerComms
    {
        public HostTrackerProxy(IOutgoingCommsLink<ITrackerComms> link)
            : base(link)
        {

        }

        public void HostOnline(object endpoint)
        {
            Link.InvokeMethod(new[] { endpoint });
        }

        public void HostOffline(object endpoint)
        {
            Link.InvokeMethod(new[] { endpoint });
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            return Link.InvokeMethod(null) as IReadOnlyList<IJobDefinition>;
        }


        public IReadOnlyList<IProcessJobValueField> SubmitJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs)
        {
            return Link.InvokeMethod(new object[] { definition.ToFlattened(), inputs }) as IReadOnlyList<IProcessJobValueField>;
        }
    }

    public sealed class Tracker
        : ITrackerComms
    {
        private readonly IIncomingCommsLink<ITrackerComms> _link;

        private Dictionary<object, HostExposerProxy> _hosts = new Dictionary<object, HostExposerProxy>();

        public Tracker(IIncomingCommsLink<ITrackerComms> incomingLink)
        {
            _link = incomingLink;
            _link.StartListening(this);
        }

        public void HostOnline(object endpoint)
        {
            _hosts.Add(endpoint,
                new HostExposerProxy(_link.CreateOutgoingOfSameTransportDiffContract<IHostComms>(endpoint)));
        }

        public void HostOffline(object endpoint)
        {
            _hosts.Remove(endpoint);
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            var lst = new List<IJobDefinition>();
            lock (_hosts)
            {
                foreach (var h in _hosts.Values)
                {
                    var jds = h.GetJobDefinitions();
                    foreach (var jd in jds)
                    {
                        if (!lst.Any(j => j.Match(jd)))
                        {
                            lst.Add(jd);
                        }
                    }
                }
            }

            return lst;
        }


        public IReadOnlyList<IProcessJobValueField> SubmitJob(IJobDefinition definition, IEnumerable<IProcessJobValueField> inputs)
        {
            var md = GetJobDefinitions().SingleOrDefault(d => d.Match(definition));

            if (md == null)
            {
                throw new InvalidOperationException("No job definition exists on hosts");
            }

            lock (_hosts)
            {
                var minload = _hosts.Values.Where(h => h.GetJobDefinitions().Any(j => j.Match(definition))).OrderBy<IHostComms, int>(h => h.QueuedJobCount()).First();
                return minload.ProcessJob(md, inputs);
            }
        }
    }

    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();

            //p.DoProcessNodeTest();

            p.NaiveDistribTest();


            Console.ReadLine();
        }

        private void NaiveDistribTest()
        {
            var nboot = new NinjectBootstrapper();
            nboot.Start();

            var trackerEndpoint = new TCPEndpointDetails()
            {
                Address = IPAddress.Loopback,
                Port = 8080,
            };

            //var trackerEndpoint = new NamedPipeEndpointDetails()
            //{
            //    MachineName = ".",
            //    PipeName = "tracker",
            //};

            var dBridge = new DirectInvokeCommsBridge("tracker bridge");

            //var track = new Tracker(
            //    new TcpIncomingCommsLink<ITrackerComms>(
            //        trackerEndpoint,
            //        new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
            //        new DirectInvocationCommsMessageProcessor()));

            //var track = new Tracker(
            //    new NamedPipeIncomingCommsLink<ITrackerComms>(
            //        trackerEndpoint,
            //        new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
            //        new DirectInvocationCommsMessageProcessor()));

            var track = new Tracker(
                new TcpIncomingCommsLink<ITrackerComms>(trackerEndpoint,
                    new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()), new DirectInvocationCommsMessageProcessor()));

            var hosts = CreateHosts(3, nboot, trackerEndpoint);

            foreach (var h in hosts)
            {
                h.Initialise();
            }

            var allDefs = track.GetJobDefinitions();

            var sw = new Stopwatch();
            sw.Start();
            var res = track.SubmitJob(allDefs.First(),
                JobDataHelper<IStockInput<string>>
                .New(allDefs.First())
                .ForInputSet()
                .Set(i => i.Input, "Clint")
                .ToValueFields());
            sw.Stop();
            var el = sw.Elapsed;
            Console.WriteLine(el.Milliseconds);
        }

        private IEnumerable<HostExposer> CreateHosts(int num, IIOC ioc, TCPEndpointDetails trackerEndpoint)
        {
            int startPort = 8080;

            for (int i = 1; i <= num; i++)
            {
                yield return new HostExposer(ioc.Get<IProcessHostFactory>().CreateHostFromType(typeof(SomeProcess)),
                    //new DirectInvokeIncomingCommsLink<IHostComms>(new DirectInvokeCommsBridge(Guid.NewGuid().ToString())),
                    //new HostTrackerProxy(new DirectInvokeOutgoingCommsLink<ITrackerComms>(trackerBridge)));
                    //new NamedPipeIncomingCommsLink<IHostComms>(new NamedPipeEndpointDetails()
                    //{
                    //    MachineName = ".",
                    //    PipeName = Guid.NewGuid().ToString(),
                    //}, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()), new DirectInvocationCommsMessageProcessor()),
                    //new HostTrackerProxy(
                    //    new NamedPipeOutgoingCommsLink<ITrackerComms>(trackerEndpoint.MachineName, trackerEndpoint.PipeName,
                    //        new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()))));
                    new TcpIncomingCommsLink<IHostComms>(new TCPEndpointDetails()
                    {
                        Address = IPAddress.Loopback,
                        Port = startPort + (i * 10),
                    }, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()), new DirectInvocationCommsMessageProcessor()),
                    new HostTrackerProxy(
                        new TcpOutgoingCommsLink<ITrackerComms>(trackerEndpoint.Address, trackerEndpoint.Port, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()))));
            }
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

            var cp = new CommsActionParameter()
            {
                Name = actArg.Name,
                Types = actArg.ParameterType.GenericTypeArguments.Select(t => t.FullName).ToArray(),
                Values = vlist.ToArray(),
            };


            var s = "";
        }

        public sealed class CommsActionParameter
        {
            public string Name { get; set; }
            public string[] Types { get; set; }
            public object[] Values { get; set; }
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