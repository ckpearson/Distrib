using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Concurrent;
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

        private ConcurrentQueue<KeyValuePair<Action<Action<string>>, Func<string>>> _visibleTasks =
            new ConcurrentQueue<KeyValuePair<Action<Action<string>>, Func<string>>>();
        private Task _visibleTaskRunner = null;

        private volatile bool _visibleTaskExecuting = false;

        public void PerformVisibleTask(Action<Action<string>> taskAction, Func<string> finishedAction = null)
        {
            if (_visibleTaskRunner == null)
            {
                _visibleTaskRunner = Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(300);
                        if (_visibleTasks.Count == 0)
                        {
                            continue;
                        }

                        KeyValuePair<Action<Action<string>>, Func<string>> tskDetails;
                        if (_visibleTasks.TryDequeue(out tskDetails))
                        {
                            _visibleTaskExecuting = true;
                            _eventAggregator.GetEvent<Events.ApplicationTaskRunningEvent>()
                                .Publish(_visibleTasks.Count);
                            BusyInProgress();
                            this.StatusText = null;

                            tskDetails.Key((s) => this.StatusText = s);

                            BusyFinished();
                            if (tskDetails.Value != null)
                            {
                                this.StatusText = tskDetails.Value();
                            }

                            Thread.Sleep(1500);
                            this.StatusText = null;

                            _visibleTaskExecuting = false;
                            _eventAggregator.GetEvent<Events.ApplicationTaskFinishedEvent>()
                                .Publish(_visibleTasks.Count);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                });
            }

            _eventAggregator.GetEvent<Events.ApplicationTaskQueuedEvent>()
                .Publish(_visibleTasks.Count + 1);
            _visibleTasks.Enqueue(new KeyValuePair<Action<Action<string>>, Func<string>>(taskAction, finishedAction));
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            private set
            {
                _statusText = value;
                _eventAggregator.GetEvent<Events.AppStatusTextChangedEvent>()
                    .Publish(_statusText);
            }
        }


       
    }
}
