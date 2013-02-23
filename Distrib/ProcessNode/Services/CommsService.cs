using Distrib.Communication;
using Distrib.Nodes.Process;
using ProcessNode.Events;
using ProcessNode.Models;
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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Services
{
    [Export(typeof(ICommsService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class CommsService : ICommsService
    {
        private IIncomingCommsLink<IProcessNodeComms> _link;
        private IProcessNode _processNode;

        private IReadOnlyList<ConnectionDetails> _connectionDetails;

        private IDistribAccessService _distrib;

        private INewEventAggregator _eventAgg;

        private IAppStateService _appState;

        [ImportingConstructor()]
        public CommsService(IDistribAccessService distrib, INewEventAggregator eventAgg, IAppStateService appState)
        {
            _distrib = distrib;
            _eventAgg = eventAgg;
            _appState = appState;
        }

        private readonly object _lock = new object();

        public bool IsListening
        {
            get
            {
                lock (_lock)
                {
                    if (_link == null)
                    {
                        return false;
                    }
                    else
                    {
                        lock (_link)
                        {
                            return _link.IsListening;
                        }
                    } 
                }
            }
        }

        public IEnumerable<ConnectionDetails> AvailableConnectionDetails
        {
            get
            {
                lock (_lock)
                {
                    if (_connectionDetails == null)
                    {
                        _connectionDetails = new List<ConnectionDetails>()
                    {
                        new TcpConnectionDetails(),
                        new NamedPipeConnectionDetails(),
                    }.AsReadOnly();
                    }

                    return _connectionDetails; 
                }
            }
        }


        public ConnectionDetails GetConnectionDetailsForType(ConnectionType type)
        {
            lock (_lock)
            {
                return _connectionDetails.SingleOrDefault(d => d.Type == type); 
            }
        }


        public void StartListening(ConnectionDetails details)
        {
            if (details == null) throw new ArgumentException("Details must be supplied");

            try
            {
                lock (_lock)
                {
                    if (_link != null)
                    {
                        throw new InvalidOperationException("A link already exists on the comms service");
                    }

                    if (_processNode != null)
                    {
                        _processNode = null;
                    }

                    _link = details.CreateIncomingLink<IProcessNodeComms>();
                    _processNode = _distrib.Container.Get<IProcessNodeFactory>()
                        .Create(_link);


                    this.ListeningDetails = details;

                    _eventAgg.Send(new Events.NodeListeningChangedEvent(true));
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
                    if (_link == null)
                    {
                        throw new InvalidOperationException("Not currently listening");
                    }

                    _link.StopListening();
                    _link = null;
                    _processNode = null;
                    this.ListeningDetails = null;
                    _eventAgg.Send(new Events.NodeListeningChangedEvent(false));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to stop listening", ex);
            }
        }


        public ConnectionDetails ListeningDetails
        {
            get;
            private set;
        }
    }
}
