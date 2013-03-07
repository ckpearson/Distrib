using Distrib.Nodes.Process;
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
    public sealed class CommsEditViewModel :
        ViewModelBase
    {
        private readonly INodeHostingService _nodeHosting;
        private readonly INewEventAggregator _eventAgg;

        private readonly IReadOnlyList<ICommsProvider<IProcessNodeComms>> _commsProviders;

        [ImportingConstructor]
        public CommsEditViewModel(
            INodeHostingService nodeHostingService,
            INewEventAggregator eventAgg,
            [ImportMany]IEnumerable<ICommsProvider<IProcessNodeComms>> commsProviders)
            : base(false)
        {
            _nodeHosting = nodeHostingService;
            _eventAgg = eventAgg;

            _commsProviders = commsProviders.ToList().AsReadOnly();
            if (_commsProviders.Count > 0)
            {
                this.SelectedEndpoint = Endpoints.First();
            }
        }

        private IReadOnlyList<CommsEndpointDetails> _endpoints;

        public IReadOnlyList<CommsEndpointDetails> Endpoints
        {
            get
            {
                if (_endpoints == null)
                {
                    _endpoints = _commsProviders.Select(p => p.GetEndpointDetailsItem()).ToList().AsReadOnly();
                }

                return _endpoints;
            }
        }

        public bool CanStartListening
        {
            get
            {
                if (_selectedEndpoint == null)
                {
                    return false;
                }

                return _selectedEndpoint.GetFields().All(f => string.IsNullOrEmpty(f["Value"]));
            }
        }

        private DelegateCommand _startListeningCommand;
        public ICommand StartListeningCommand
        {
            get
            {
                if (_startListeningCommand == null)
                {
                    _startListeningCommand = new DelegateCommand(() =>
                        {
                            _nodeHosting.StartListening();
                        });
                }

                return _startListeningCommand;
            }
        }

        private CommsEndpointDetails _selectedEndpoint;
        public CommsEndpointDetails SelectedEndpoint
        {
            get { return _selectedEndpoint; }
            set
            {
                if (_selectedEndpoint != null)
                {
                    foreach (var fld in SelectedEndpointFields)
                    {
                        fld.PropertyChanged -= OnEndpointFieldChanged;
                    }
                }

                _selectedEndpoint = value;
                foreach (var fld in SelectedEndpointFields)
                {
                    fld.PropertyChanged += OnEndpointFieldChanged;
                }
                PropChanged();
                PropChanged("SelectedEndpointFields");
            }
        }

        private void OnEndpointFieldChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropChanged("CanStartListening");
        }

        public IEnumerable<CommsEndpointDetailsField> SelectedEndpointFields
        {
            get
            {
                if (_selectedEndpoint == null)
                {
                    return null;
                }
                else
                {
                    return _selectedEndpoint.GetFields();
                }
            }
        }
    }
}
