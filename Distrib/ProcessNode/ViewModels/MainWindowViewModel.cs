using Distrib.Nodes.Process;
using DistribApps.Core.Events;
using DistribApps.Core.Services;
using DistribApps.Core.ViewModels;
using DistribApps.Core.ViewServices;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class MainWindowViewModel :
        ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;

        private ViewModelBase _activeViewModel;

        [ImportingConstructor]
        public MainWindowViewModel(INewEventAggregator eventAgg)
            : base(false)
        {
            _eventAgg = eventAgg;

            _eventAgg.Subscribe<ViewBecameActiveEvent>(OnViewBecameActive);
        }

        private void OnViewBecameActive(ViewBecameActiveEvent ev)
        {
            // Take out the handlers for the current model if one is set
            if (_activeViewModel != null)
            {
                _activeViewModel.CanRefreshChanged -= OnViewCanRefreshChanged;
            }

            _activeViewModel = ev.ViewModel;
            _activeViewModel.CanRefreshChanged += OnViewCanRefreshChanged;
            PropChanged("CanViewRefresh");
        }

        private void OnViewCanRefreshChanged(IRefreshable refreshableView, bool canRefresh)
        {
            PropChanged("CanViewRefresh");
        }

        public bool CanViewRefresh
        {
            get
            {
                if (_activeViewModel == null)
                {
                    return false;
                }
                else
                {
                    return _activeViewModel.CanRefresh;
                }
            }
        }

        private DelegateCommand _doRefreshCommand;
        public ICommand DoRefreshCommand
        {
            get
            {
                if (_doRefreshCommand == null)
                {
                    _doRefreshCommand = new DelegateCommand(() =>
                        {
                            if (_activeViewModel != null)
                            {
                                _activeViewModel.Refresh();
                            }
                        });
                }

                return _doRefreshCommand;
            }
        }
    }
}
