using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.ViewModels
{
    [Export()]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class BusyIndicatorViewModel : ViewModelBase
    {
        [ImportingConstructor()]
        public BusyIndicatorViewModel(IEventAggregator eventAgg)
        {
            eventAgg.GetEvent<Events.ApplicationBusyChangedEvent>().Subscribe(OnApplicationBusyEvent);
        }

        private bool _busy = false;
        public bool IsBusy
        {
            get { return _busy; }
            set
            {
                _busy = value;
                OnPropertyChanged();
            }
        }

        private void OnApplicationBusyEvent(bool isBusy)
        {
            this.IsBusy = isBusy;
        }
    }
}
