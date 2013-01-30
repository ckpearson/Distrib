using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessRunner.Services
{
    [Export(typeof(IAppStateService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class AppStateService : IAppStateService
    {
        private IEventAggregator _eventAggregator;

        [ImportingConstructor()]
        public AppStateService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        private long _iBusyActionCount;

        public bool AppBusy
        {
            get
            {
                return Interlocked.Read(ref _iBusyActionCount) > 0;
            }
        }

        private void BusyInProgress()
        {
            Interlocked.Increment(ref _iBusyActionCount);
            _eventAggregator.GetEvent<Events.ApplicationBusyChangedEvent>().Publish(true);
        }

        private void BusyFinished()
        {
            Interlocked.Decrement(ref _iBusyActionCount);
            if (Interlocked.Read(ref _iBusyActionCount) == 0)
            {
                _eventAggregator.GetEvent<Events.ApplicationBusyChangedEvent>().Publish(false);
            }
        }

        public void DoAsBusy(Action act, Action actFinished = null)
        {
            BusyInProgress();
            Task.Factory.StartNew(() =>
                {
                    act();
                }).ContinueWith((t) =>
                {
                    BusyFinished();
                    if (actFinished != null)
                    {
                        actFinished();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
