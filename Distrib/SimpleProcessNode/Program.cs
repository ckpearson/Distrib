using Distrib.IOC.Ninject;
using Distrib.Nodes.Process;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.Communication;
using System.Net;
using RemoteProcessing.Shared;
using Distrib.Processes.Stock;
using System.Runtime.CompilerServices;

namespace SimpleProcessNode
{
    class Program :
        ISimpleProcNode_Comms
    {
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        private IIncomingCommsLink<ISimpleProcNode_Comms> _incoming;

        private IProcessHost _host;

        private void Run()
        {
            var nboot = new NinjectBootstrapper();
            nboot.Start();

            Console.WriteLine("Starting incoming comms...");

            _incoming = new TcpIncomingCommsLink<ISimpleProcNode_Comms>(
                new TCPEndpointDetails()
                {
                    Address = IPAddress.Loopback,
                    Port = 8080,
                }, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
                new DirectInvocationCommsMessageProcessor());

            _incoming.StartListening(this);
            Console.WriteLine("Now listening on port 8080...");

            Console.WriteLine("Initialising host...");
            _host = nboot.Get<IProcessHostFactory>()
                .CreateHostFromType(typeof(SimpleProcess));
            _host.Initialise();

            Console.WriteLine("Host initialised, process details:");
            Console.WriteLine("\t{0}", _host.Metadata.Name);
            Console.WriteLine("\t{0}", _host.Metadata.Description);
            Console.WriteLine("\t{0}", _host.Metadata.Version.ToString("#.#"));
            Console.WriteLine();

            Console.WriteLine("<ENTER> to terminate");
            Console.ReadLine();

            _incoming.StopListening();
            _incoming = null;
            _host.Unitialise();
            _host = null;
        }

        IReadOnlyList<IJobDefinition> ISimpleProcNode_Comms.GetJobDefinitions()
        {
            echoRequest();
            if (_host != null && _host.IsInitialised)
            {
                return _host.JobDefinitions.Select(jd => jd.ToFlattened()).ToArray();
            }

            throw new InvalidOperationException("Host not initialised");
        }

        private void echoRequest([CallerMemberName] string name = "")
        {
            Console.WriteLine("[REMOTE REQUEST] - {0}", name);
        }


        string ISimpleProcNode_Comms.SayHello(string name)
        {
            echoRequest();
            if (_host != null && _host.IsInitialised)
            {
                var jd = _host.JobDefinitions.First(j => j.Name == "Say Hello");

                return JobDataHelper<IStockOutput<string>>.New(jd)
                    .ForOutputGet(
                        _host.QueueJobAndWait(jd,
                            JobDataHelper<IStockInput<string>>.New(jd)
                            .ForInputSet()
                            .Set(i => i.Input, name).ToValueFields())).Get(o => o.Output);
            }

            throw new InvalidOperationException("Host not initialised");
        }
    }

    [Distrib.Processes.TypePowered.ProcessMetadata("Simple process",
        "Stupidly simple process",
        1.0,
        "Clint Pearson")]
    public sealed class SimpleProcess :
        IProcess
    {

        public void InitProcess()
        {
        }

        public void UninitProcess()
        {
        }

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get
            {
                return new[]
                {
                    new ProcessJobDefinition<IStockInput<string>, IStockOutput<string>>(
                        "Say Hello",
                        "Says hello"),
                };
            }
        }

        public void ProcessJob(IJob job)
        {
            JobExecutionHelper.New()
                .AddHandler(() => JobDefinitions.First(),
                () =>
                {
                    JobDataHelper<IStockOutput<string>>.New(job.Definition)
                        .ForOutputSet(job)
                        .Set(o => o.Output,
                            string.Format("Hello {0}", JobDataHelper<IStockInput<string>>.New(job.Definition)
                        .ForInputGet(job)
                        .Get(i => i.Input)));
                }).Execute(job.Definition);
        }
    }


}
