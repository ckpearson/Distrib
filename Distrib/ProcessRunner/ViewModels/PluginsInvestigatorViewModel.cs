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
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class PluginsInvestigatorViewModel : ViewModelBase
    {
        private IAppStateService _appState;
        private IDistribInteractionService _distrib;
        private IEventAggregator _eventAgg;

        [ImportingConstructor()]
        public PluginsInvestigatorViewModel(IAppStateService appState, IDistribInteractionService distrib, IEventAggregator eventAgg)
        {
            _appState = appState;
            _distrib = distrib;
            _eventAgg = eventAgg;

            this.Plugins = new ObservableCollection<Models.DistribPlugin>();

            _eventAgg.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Subscribe(OnAssemblyStateChanged, ThreadOption.UIThread);
        }

        private void OnAssemblyStateChanged(Events.PluginAssemblyStateChange state)
        {
            if (state == Events.PluginAssemblyStateChange.AssemblyInitialised)
            {
                Plugins.Clear();
                foreach (var plugin in _distrib.CurrentAssembly.Plugins)
                {
                    this.Plugins.Add(plugin);
                }
            }
            else if (state == Events.PluginAssemblyStateChange.AssemblyUninitialised)
            {
                Plugins.Clear();
            }
        }

        public ObservableCollection<Models.DistribPlugin> Plugins
        {
            get;
            private set;
        }
    }
}
