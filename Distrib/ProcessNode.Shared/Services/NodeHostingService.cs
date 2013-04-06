using Distrib.Communication;
using Distrib.IOC;
using Distrib.Nodes.Process;
using Distrib.Processes;
using DistribApps.Comms;
using DistribApps.Core.Events;
using DistribApps.Core.Processes.Hosting;
using DistribApps.Core.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessNode.Shared.Services
{
    
    internal sealed class ProcNode :
        IProcessNode, 
        IManagedProcessNode
    {
        private readonly IIncomingCommsLink<IProcessNodeComms> _link;
        private readonly CommsComponent _comms = null;

        private ObservableCollection<IManagedProcessHost> _hosts = new ObservableCollection<IManagedProcessHost>();

        private readonly object _lock = new object();

        private readonly IProcessHostFactory _hostFactory;

        public ProcNode(
            [IOC(true)] IProcessHostFactory hostFactory,
            [IOC(false)] IIncomingCommsLink<IProcessNodeComms> incomingLink)
        {
            _hostFactory = hostFactory;
            _link = incomingLink;
            _link.StartListening(_comms = new CommsComponent(this));
        }


        private sealed class CommsComponent
            : IProcessNodeComms
        {
            private readonly ProcNode _node;

            public CommsComponent(ProcNode node)
            {
                _node = node;
            }

            public int CurrentHostCount()
            {
                throw new NotImplementedException();
            }

            public IReadOnlyList<Distrib.Processes.IJobDefinition> GetJobDefinitions()
            {
                throw new NotImplementedException();
            }

            public IReadOnlyList<Distrib.Processes.IProcessMetadata> GetProcessesMetadata()
            {
                throw new NotImplementedException();
            }

            public IReadOnlyList<Distrib.Processes.IJobDefinition> GetJobDefinitionsForProcess(Distrib.Processes.IProcessMetadata metadata)
            {
                throw new NotImplementedException();
            }
        }


        void IProcessNode.CreateAndHost(Type processType)
        {
            _hosts.Add(new ManagedProcessHost(_hostFactory.CreateHostFromType(processType)));
        }

        void IProcessNode.CreateAndHost(Distrib.Plugins.IPluginDescriptor processPluginDescriptor)
        {
            throw new NotImplementedException();
        }


        public IProcessNode CoreNode
        {
            get { return this; }
        }

        public ObservableCollection<IManagedProcessHost> Hosts
        {
            get
            {
                return _hosts;
            }
        }

        public void AddHost(IManagedProcessHost host)
        {
            this.Hosts.Add(host);
        }
    }

    public sealed class ManagedProcessHost
        : IManagedProcessHost
    {
        private readonly IProcessHost _host;

        public ManagedProcessHost(IProcessHost host)
        {
            _host = host;
        }

        public IProcessHost CoreHost
        {
            get { return _host; }
        }


        public IProcessMetadata Metadata
        {
            get { return _host.Metadata; }
        }
    }

    [Export(typeof(INodeHostingService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class NodeHostingService : 
        INodeHostingService
    {
        private readonly INewEventAggregator _eventAgg;
        private readonly IDistribAccessService _distrib;

        [ImportingConstructor]
        public NodeHostingService(
            INewEventAggregator eventAgg,
            IDistribAccessService distrib)
        {
            _eventAgg = eventAgg;
            _distrib = distrib;

            ServiceLocator.Current.GetInstance<IDistribAccessService>()
                .DistribIOC.Rebind<IProcessNode, ProcNode>(false);
        }

        private readonly object _lock = new object();

        private IIncomingCommsLink<IProcessNodeComms> _link;

        private IManagedProcessNode _node;

        private CommsEndpointDetails _currentEndpoint;


        public bool IsListening
        {
            get
            {
                lock (_lock)
                {
                    return (_link == null) ? false : _link.IsListening;
                }
            }
        }

        public CommsEndpointDetails CurrentEndpoint
        {
            get
            {
                lock (_lock)
                {
                    return _currentEndpoint;
                }
            }

            private set
            {
                lock (_lock)
                {
                    _currentEndpoint = value;
                }
            }
        }

        public void StartListening(ICommsProvider<IProcessNodeComms> provider, CommsEndpointDetails endpoint)
        {
            try
            {
                lock (_lock)
                {
                    if (IsListening)
                    {
                        throw new InvalidOperationException("Already listening");
                    }

                    if (_link != null)
                    {
                        _link = null;
                    }

                    if (_node != null)
                    {
                        // Do node cleanup here
                        _node = null;
                    }

                    _currentEndpoint = endpoint;

                    // Get the incoming comms link
                    _link = provider.CreateIncomingLink(endpoint);

                    // Create the new node
                    _node = (ProcNode)_distrib.DistribIOC.Get<IProcessNodeFactory>()
                        .Create(_link);

                    // Now the node and link are all set up and going, signal that we're up and running

                    _eventAgg.Send(new Shared.Events.NodeListeningChangedEvent(true));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to start listening", ex);
            }
        }

        public void StopListening()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsListening)
                    {
                        throw new InvalidOperationException("Not currently listening");
                    }

                    if (_link != null)
                    {
                        _link.StopListening();
                        _link = null;
                    }

                    if (_node != null)
                    {
                        _node = null;
                    }

                    _currentEndpoint = null;

                    _eventAgg.Send(new Shared.Events.NodeListeningChangedEvent(false));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to stop listening", ex);
            }
        }

        public IManagedProcessNode Node
        {
            get
            {
                lock (_lock)
                {
                    return _node;
                }
            }
        }

        [ImportMany(typeof(IHostProvider))]
        private IEnumerable<IHostProvider> _providers;

        public IEnumerable<IHostProvider> HostProviders
        {
            get { return _providers; }
        }
    }
}
