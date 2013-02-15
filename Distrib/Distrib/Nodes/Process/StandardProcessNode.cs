using Distrib.IOC;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Nodes.Process
{


    public interface IProcessNodeProcessFactory
    {
        IProcessNodeProcess Create(IProcessHost host,
            IProcessNode node);
    }

    public sealed class TempRegistrar : IOC.IOCRegistrar
    {
        public override void PerformBindings()
        {
            BindSingleton<IProcessNodeProcessFactory, ProcessNodeProcessFactory>();
            Bind<IProcessNodeProcess, ProcessNodeProcess>();
        }
    }

    public class ProcessNodeProcessFactory : IProcessNodeProcessFactory
    {
        private IIOC _ioc;

        public ProcessNodeProcessFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IProcessNodeProcess Create(IProcessHost host, IProcessNode node)
        {
            return _ioc.Get<IProcessNodeProcess>(new[]
            {
                new IOCConstructorArgument("host", host),
                new IOCConstructorArgument("node", node),
            });
        }
    }

    public sealed class StandardProcessNode : CrossAppDomainObject, IProcessNode
    {
        private readonly IProcessHostTypeService _typeService;
        private readonly IProcessHostSource _hostSource;
        private readonly IProcessNodeProcessFactory _nodeProcessFactory;

        private readonly object _lock = new object();

        private List<IProcessNodeProcess> _processes = new List<IProcessNodeProcess>();

        public StandardProcessNode(
            [IOC(true)] IProcessNodeProcessFactory nodeProcessFactory,
            [IOC(true)] IProcessHostTypeService typeService,
            [IOC(false)] IProcessHostSource hostSource)
        {
            _nodeProcessFactory = nodeProcessFactory;
            _typeService = typeService;
            _hostSource = hostSource;

            _buildInitialProcList();
        }

        private void _buildInitialProcList()
        {
            lock (_lock)
            {
                _processes.Clear();
                foreach (var procHost in _hostSource.GetHosts())
                {
                    _processes.Add(_nodeProcessFactory.Create(
                        procHost, this));
                }
            }
        }

        public IEnumerable<IProcessNodeProcess> Processes
        {
            get
            {
                lock (_lock)
                {
                    return _processes;
                }
            }
        }


        public IEnumerable<IJobDefinition> GetJobDefinitions(
            Func<IProcessNodeProcess, IEnumerable<IJobDefinition>> visitor)
        {
            try
            {
                List<IJobDefinition> defs = new List<IJobDefinition>();
                lock (_lock)
                {
                    foreach (var proc in Processes)
                    {
                        if (visitor != null)
                        {
                            try
                            {
                                foreach (var def in visitor(proc))
                                {
                                    defs.Add(def);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException("An exception occurred when working with the visitor", ex);
                            }
                        }
                        else
                        {
                            if (proc.IsInitialised)
                            {
                                foreach (var def in proc.JobDefinitions)
                                {
                                    defs.Add(def);
                                }
                            }
                        }
                    }
                }

                return defs;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get job definitions", ex);
            }
        }

        private Task _nodeTask = null;
        private CancellationTokenSource _nodeTaskCancellationToken;
        private readonly object _runnerLock = new object();

        public bool IsRunning
        {
            get
            {
                lock (_lock)
                {
                    lock (_runnerLock)
                    {
                        if (_nodeTaskCancellationToken != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public void Run()
        {
            try
            {
                lock (_lock)
                {
                    lock (_runnerLock)
                    {
                        if (IsRunning)
                        {
                            throw new InvalidOperationException("Already running");
                        }

                        _nodeTaskCancellationToken = new CancellationTokenSource();
                        _nodeTask = Task.Factory.StartNew(__NodeRunner, _nodeTaskCancellationToken.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to run", ex);
            }
        }

        private void __NodeRunner()
        {
            // Do startup tasks here


            // Enter primary loop
            while (true)
            {
                lock (_runnerLock)
                {
                    if (_nodeTaskCancellationToken == null || _nodeTaskCancellationToken.IsCancellationRequested)
                    {
                        // Do any cleanup needed
                        break;
                    }
                }

                // Do any runner tasks here


                // Wait so this background process doesn't consume insane amounts of CPU
                Thread.Sleep(250);
            }

            // Task being stopped, clean up behind ready for next
            _nodeTaskCancellationToken = null;
            _nodeTask = null;
        }

        public void Stop()
        {
            try
            {
                lock (_lock)
                {
                    lock (_runnerLock)
                    {
                        if (!IsRunning)
                        {
                            throw new InvalidOperationException("Not currently running");
                        }

                        if (_nodeTaskCancellationToken.IsCancellationRequested)
                        {
                            throw new InvalidOperationException("Stop already requested");
                        }
                        else
                        {
                            _nodeTaskCancellationToken.Cancel();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to stop", ex);
            }
        }
    }

    public interface IProcessNodeProcess
    {
        SystemPowerType PowerType { get; }
        bool IsInitialised { get; }
        IReadOnlyList<IJobDefinition> JobDefinitions { get; }
        void Initialise();
        void Uninitialise();
    }

    public class ProcessNodeProcess : IProcessNodeProcess
    {
        private readonly IProcessHost _host;
        private readonly IProcessNode _node;
        private readonly IProcessHostTypeService _typeService;
        private readonly SystemPowerType _powerType;

        private readonly object _lock = new object();

        public ProcessNodeProcess(
            [IOC(false)] IProcessHost host,
            [IOC(false)] IProcessNode node,
            [IOC(true)] IProcessHostTypeService typeService)
        {
            _host = host;
            _node = node;
            _typeService = typeService;
            _powerType = _typeService.GetPowerType(_host);
        }

        public SystemPowerType PowerType
        {
            get { return _powerType; }
        }

        public bool IsInitialised
        {
            get
            {
                lock (_lock)
                {
                    return _host.IsInitialised;
                }
            }
        }

        public IReadOnlyList<IJobDefinition> JobDefinitions
        {
            get
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        return null;
                    }
                    else
                    {
                        return _host.JobDefinitions;
                    }
                }
            }
        }

        public void Initialise()
        {
            lock (_lock)
            {
                _host.Initialise();
            }
        }

        public void Uninitialise()
        {
            lock (_lock)
            {
                _host.Unitialise();
            }
        }
    }

    public interface IProcessHostSource
    {
        IEnumerable<IProcessHost> GetHosts();
    }

    namespace HostSources
    {
        public sealed class KnownProcessHostSource : IProcessHostSource
        {
            private readonly IEnumerable<IProcessHost> _hosts;

            public KnownProcessHostSource(IEnumerable<IProcessHost> hosts)
            {
                _hosts = hosts;
            }

            public IEnumerable<IProcessHost> GetHosts()
            {
                return _hosts;
            }
        }

        public sealed class AggregateProcessHostSource : IProcessHostSource
        {
            private readonly IEnumerable<IProcessHostSource> _sources;

            public AggregateProcessHostSource(IEnumerable<IProcessHostSource> sources)
            {
                _sources = sources;
            }

            public IEnumerable<IProcessHost> GetHosts()
            {
                return _sources.SelectMany(s => s.GetHosts());
            }
        }
    }

}
