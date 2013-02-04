/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
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

                            Thread.Sleep(2500);
                            this.StatusText = null;

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
