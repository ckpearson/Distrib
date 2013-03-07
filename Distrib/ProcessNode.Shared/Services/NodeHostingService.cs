using DistribApps.Core.Events;
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

        [ImportingConstructor]
        public NodeHostingService(
            INewEventAggregator eventAgg)
        {
            _eventAgg = eventAgg;
        }

        private bool _isListening = false;
        public bool IsListening
        {
            get { return _isListening; }
        }

        public void StartListening()
        {
            _isListening = true;
            _eventAgg.Send(new Shared.Events.NodeListeningChangedEvent(true));
        }

        public void StopListening()
        {
            _isListening = false;
            _eventAgg.Send(new Shared.Events.NodeListeningChangedEvent(false));
        }
    }
}
