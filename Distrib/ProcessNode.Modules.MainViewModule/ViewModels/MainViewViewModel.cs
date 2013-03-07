using DistribApps.Core.Events;
using DistribApps.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessNode.Modules.MainViewModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class MainViewViewModel
        : ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;

        [ImportingConstructor]
        public MainViewViewModel(INewEventAggregator eventAgg)
            : base(false)
        {
            _eventAgg = eventAgg;

            base.IsActiveChanged += MainViewViewModel_IsActiveChanged;
        }

        void MainViewViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (this.IsActive)
            {
                // Let subscribers know we're not the active view
                _eventAgg.Send(new ViewBecameActiveEvent(this));
            }
        }
    }
}
