using DistribApps.Core.Events;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using ProcessNode.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Modules.HostModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class HostsListViewViewModel : 
        ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;
        private readonly INodeHostingService _hosting;

        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public HostsListViewViewModel(
            INewEventAggregator eventAgg,
            INodeHostingService hosting,
            IRegionManager regionManager)
            : base(false)
        {
            _eventAgg = eventAgg;
            _hosting = hosting;
            _regionManager = regionManager;

            _eventAgg.Subscribe<Shared.Events.NodeListeningChangedEvent>(OnNodeListeningChanged);
        }

        private void OnNodeListeningChanged(Shared.Events.NodeListeningChangedEvent ev)
        {
            PropChanged("IsListening");
            if (ev.IsListening)
            {
                _regionManager.Regions[Regions.NodeListening].Add(ServiceLocator.Current.GetInstance<Views.NodeListeningHostsListView>());
            }
            else
            {
                _regionManager.Regions[Regions.NodeListening].Remove(ServiceLocator.Current.GetInstance<Views.NodeListeningHostsListView>());
            }
        }

        public bool IsListening
        {
            get { return _hosting.IsListening; }
        }
    }
}
