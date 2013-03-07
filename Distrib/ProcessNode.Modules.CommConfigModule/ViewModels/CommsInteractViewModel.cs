using DistribApps.Comms;
using DistribApps.Core.Events;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using ProcessNode.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.Modules.CommConfigModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class CommsInteractViewModel :
        ViewModelBase
    {
        private readonly INodeHostingService _nodeHosting;
        private readonly INewEventAggregator _eventAgg;

        [ImportingConstructor]
        public CommsInteractViewModel(
            INodeHostingService nodeHostingService,
            INewEventAggregator eventAgg)
            : base(false)
        {
            _nodeHosting = nodeHostingService;
            _eventAgg = eventAgg;

            _eventAgg.Subscribe<Shared.Events.NodeListeningChangedEvent>(OnNodeListeningChanged);
        }

        private CommsEndpointDetails _endpoint;
        public CommsEndpointDetails Endpoint
        {
            get { return _endpoint; }
            private set
            {
                _endpoint = value;
                PropChanged();
            }
        }

        private void OnNodeListeningChanged(Shared.Events.NodeListeningChangedEvent ev)
        {
            if (ev.IsListening)
            {
                this.Endpoint = _nodeHosting.CurrentEndpoint;
            }
            else
            {
                this.Endpoint = null;
            }
        }

        private DelegateCommand _stopListeningCommand;
        public ICommand StopListeningCommand
        {
            get
            {
                if (_stopListeningCommand == null)
                {
                    _stopListeningCommand = new DelegateCommand(() =>
                        {
                            _nodeHosting.StopListening();
                        });
                }

                return _stopListeningCommand;
            }
        }
    }
}
