using Distrib.Communication;
using Distrib.Nodes.Process;
using DistribApps.Comms;
using DistribApps.Core.Events;
using DistribApps.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Shared.Services
{
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
        }

        private readonly object _lock = new object();

        private IIncomingCommsLink<IProcessNodeComms> _link;

        private IProcessNode _node;

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
                    _node = _distrib.DistribIOC.Get<IProcessNodeFactory>()
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
    }
}
