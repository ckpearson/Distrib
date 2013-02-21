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

            _eventAgg.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Subscribe(OnPluginAssemblyStateChange);
        }

        private void OnPluginAssemblyStateChange(Events.PluginAssemblyStateChange state)
        {
            OnPropertyChanged("AssemblyLoadedAndInitialised");
        }

        public bool AssemblyLoadedAndInitialised
        {
            get { return _distribService.CurrentAssembly != null && _distribService.CurrentAssembly.Initialised; }
        }
    }
}
