using Microsoft.Practices.Prism.Events;
using ProcessRunner.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class ProcessRunnerMainViewModel : ViewModelBase
    {
        private IAppStateService _appState;
        private IDistribInteractionService _distribService;
        private IEventAggregator _eventAgg;

        [ImportingConstructor]
        public ProcessRunnerMainViewModel(Services.IDistribInteractionService distribService,
            IEventAggregator eventAggregator, Services.IAppStateService appState)
        {
            _appState = appState;
            _distribService = distribService;
            _eventAgg = eventAggregator;
        }
    }
}
