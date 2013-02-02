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
