/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
            eventAgg.GetEvent<Events.AppStatusTextChangedEvent>().Subscribe(OnApplicationStatusTextChanged);
            eventAgg.GetEvent<Events.ApplicationTaskQueuedEvent>()
                .Subscribe(OnVisibleTaskQueued);
            eventAgg.GetEvent<Events.ApplicationTaskRunningEvent>()
                .Subscribe(OnVisibleTaskRunning);
            eventAgg.GetEvent<Events.ApplicationTaskFinishedEvent>()
                .Subscribe(OnVisibleTaskFinished);
        }

        private void OnVisibleTaskFinished(int waitingTasks)
        {
            this.TaskRunning = false;
            this.TotalTasksWaiting = waitingTasks;
            OnPropertyChanged("TasksWaiting");
            OnPropertyChanged("TaskRunningWithMoreWaiting");
        }

        private void OnVisibleTaskRunning(int waitingTasks)
        {
            this.TaskRunning = true;
            this.TotalTasksWaiting = waitingTasks;
            OnPropertyChanged("TasksWaiting");
            OnPropertyChanged("TaskRunningWithMoreWaiting");
        }

        private void OnVisibleTaskQueued(int waitingTasks)
        {
            this.TotalTasksWaiting = waitingTasks;
            OnPropertyChanged("TasksWaiting");
            OnPropertyChanged("TaskRunningWithMoreWaiting");
        }

        private void OnApplicationStatusTextChanged(string obj)
        {
            this.StatusText = obj;
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

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            private set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public bool TaskRunningWithMoreWaiting
        {
            get { return TaskRunning && TasksWaiting; }
        }

        private bool _taskRunning = false;
        public bool TaskRunning
        {
            get { return _taskRunning; }
            private set
            {
                _taskRunning = value;
                OnPropertyChanged();
            }
        }

        public bool TasksWaiting
        {
            get
            {
                return TotalTasksWaiting > 0;
            }
        }

        private int _totalTasksWaiting;
        public int TotalTasksWaiting
        {
            get { return _totalTasksWaiting; }
            private set
            {
                _totalTasksWaiting = value;
                OnPropertyChanged();
            }
        }

        private void OnApplicationBusyEvent(bool isBusy)
        {
            this.IsBusy = isBusy;
        }
    }
}
