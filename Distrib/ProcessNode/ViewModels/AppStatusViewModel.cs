using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using ProcessNode.Events;
using ProcessNode.Services;
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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.ViewModels
{
    [Export()]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class AppStatusViewModel
        : ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;

        private readonly Queue<AppStatusUpdate> _statusQueue = new Queue<AppStatusUpdate>();

        [ImportingConstructor()]
        public AppStatusViewModel(INewEventAggregator eventAgg)
        {
            _eventAgg = eventAgg;
            _eventAgg.Subscribe<Events.AppStatusUpdatedEvent>((e) => OnAppStatusUpdated(e.Update),
                ThreadOption.UIThread);
        }

        private void OnAppStatusUpdated(AppStatusUpdate update)
        {
            _addUpdateToQueue(update);
        }

        private void _addUpdateToQueue(AppStatusUpdate update)
        {
            lock (_statusQueue)
            {
                _statusQueue.Enqueue(update);
                _processStatusQueue();
            }
        }

        private void _dismissCurrentStatus()
        {
            lock (_statusQueue)
            {
                this.CurrentStatus = null;
                _processStatusQueue();
            }
        }

        private void _processStatusQueue()
        {
            lock (_statusQueue)
            {
                if (this.CurrentStatus == null)
                {
                    if (_statusQueue.Count > 0)
                    {
                        this.CurrentStatus = _statusQueue.Dequeue();
                    }
                }

                PropChange("StatusCount");
            }
        }

        private AppStatusUpdate _currentStatus;
        public AppStatusUpdate CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                _currentStatus = value;
                PropChange();
                PropChange("HasStatus");
            }
        }

        public bool HasStatus
        {
            get { return _currentStatus != null; }
        }

        public int StatusCount
        {
            get { lock (_statusQueue) { return _statusQueue.Count; } }
        }

        private DelegateCommand<AppStatusUpdateInteractionOption> _invokeStatusOptionCommand;

        public ICommand InvokeStatusOptionCommand
        {
            get
            {
                if (_invokeStatusOptionCommand == null)
                {
                    _invokeStatusOptionCommand = new DelegateCommand<AppStatusUpdateInteractionOption>((o) =>
                        {
                            _dismissCurrentStatus();
                            o.Action();
                        });
                }
                return _invokeStatusOptionCommand;
            }
        }

        private DelegateCommand _dismissStatusCommand;
        public ICommand DismissStatusCommand
        {
            get
            {
                if (_dismissStatusCommand == null)
                {
                    _dismissStatusCommand = new DelegateCommand(_dismissCurrentStatus);
                }

                return _dismissStatusCommand;
            }
        }

        protected override void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext context)
        {
        }
    }
}
