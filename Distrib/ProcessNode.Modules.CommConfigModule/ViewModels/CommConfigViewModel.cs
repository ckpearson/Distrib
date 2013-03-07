using DistribApps.Core.Events;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using ProcessNode.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProcessNode.Modules.CommConfigModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class CommConfigViewModel :
        ViewModelBase
    {
        private readonly INodeHostingService _nodeHosting;
        private readonly INewEventAggregator _eventAgg;

        [ImportingConstructor]
        public CommConfigViewModel(
            INodeHostingService nodeHostingService,
            INewEventAggregator eventAgg)
            : base(false)
        {
            _nodeHosting = nodeHostingService;
            _eventAgg = eventAgg;

            _eventAgg.Subscribe<Shared.Events.NodeListeningChangedEvent>(OnNodeListeningChanged);
        }

        private void OnNodeListeningChanged(Shared.Events.NodeListeningChangedEvent ev)
        {
            PropChanged("IsListening");
        }

        public bool IsListening
        {
            get { return _nodeHosting.IsListening; }
        }
    }
}
