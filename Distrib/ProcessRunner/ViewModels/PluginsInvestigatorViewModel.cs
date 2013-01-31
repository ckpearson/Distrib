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
