using Distrib.IOC;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    if (!procHost.IsInitialised)
                    {
                        procHost.Initialise();
                    }
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
    }

    public interface IProcessNodeProcess
    {
        SystemPowerType PowerType { get; }
        bool IsInitialised { get; }
        IReadOnlyList<IJobDefinition> JobDefinitions { get; }
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
