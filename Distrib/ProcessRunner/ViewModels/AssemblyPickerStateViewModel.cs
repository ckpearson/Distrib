using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using ProcessRunner.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class AssemblyPickerStateViewModel : ViewModelBase
    {
        private IAppStateService _appState;

        [ImportingConstructor()]
        public AssemblyPickerStateViewModel(IAppStateService appState)
        {
            _appState = appState;
        }

        private bool _isBusy = false;

        private DelegateCommand _toggleBusyCommand;
        public ICommand ToggleBusyCommand
        {
            get
            {
                if (_toggleBusyCommand == null)
                {
                    _toggleBusyCommand = new DelegateCommand(() =>
                        {
                            _appState.DoAsBusy(() =>
                                {
                                    Thread.Sleep(3000);
                                });
                        });
                }

                return _toggleBusyCommand;
            }
        }
    }
}
