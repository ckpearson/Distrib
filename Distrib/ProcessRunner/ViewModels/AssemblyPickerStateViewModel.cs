using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Win32;
using ProcessRunner.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
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
        private IDistribInteractionService _distribInteraction;
        private IEventAggregator _eventAggregator;

        [ImportingConstructor()]
        public AssemblyPickerStateViewModel(IAppStateService appState, 
            IDistribInteractionService distribInteraction,
            IEventAggregator eventAggregator)
        {
            _appState = appState;
            _distribInteraction = distribInteraction;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Subscribe(OnAssemblyStateChanged);
        }

        private void OnAssemblyStateChanged(Events.PluginAssemblyStateChange stateChange)
        {
            switch (stateChange)
            {
                case Events.PluginAssemblyStateChange.AssemblyLoaded:
                    NotifyProperties();
                    break;

                case Events.PluginAssemblyStateChange.AssemblyUnloaded:
                    NotifyProperties();
                    break;

                default:
                    break;
            }
        }

        private void NotifyProperties()
        {
            OnPropertyChanged("AssemblyLoaded");
            OnPropertyChanged("AssemblyPath");
            OnPropertyChanged("AssemblyFileName");
        }

        public bool AssemblyLoaded
        {
            get
            {
                return _distribInteraction.CurrentAssembly != null;
            }
        }

        public string AssemblyFileName
        {
            get
            {
                if (_distribInteraction.CurrentAssembly != null)
                {
                    return Path.GetFileName(_distribInteraction.CurrentAssembly.Path);
                }

                return null;
            }
        }

        public string AssemblyPath
        {
            get
            {
                if (_distribInteraction.CurrentAssembly != null)
                {
                    return _distribInteraction.CurrentAssembly.Path;
                }

                return null;
            }
        }

        private DelegateCommand _loadAssemblyCommand;
        public ICommand LoadAssemblyCommand
        {
            get
            {
                if (_loadAssemblyCommand == null)
                {
                    _loadAssemblyCommand = new DelegateCommand(() =>
                        {
                            _appState.PerformVisibleTask((updateStatus) =>
                                {
                                    var ofd = new OpenFileDialog();
                                    ofd.Title = "Choose a plugin assembly";
                                    ofd.Filter = "Assemblies|*.dll";
                                    ofd.Multiselect = false;
                                    if (ofd.ShowDialog().Value)
                                    {
                                        updateStatus("Loading assembly...");
                                        _distribInteraction.LoadAssembly(ofd.FileName);
                                        updateStatus("Assembly loaded");
                                    }
                                    else
                                    {
                                        updateStatus("Assembly load cancelled by user");
                                    }
                                });
                        },() => !_appState.AppBusy);
                }

                return _loadAssemblyCommand;
            }
        }

        private DelegateCommand _unloadAssemblyCommand;
        public ICommand UnloadAssemblyCommand
        {
            get
            {
                if (_unloadAssemblyCommand == null)
                {
                    _unloadAssemblyCommand = new DelegateCommand(() =>
                        {
                            _appState.PerformVisibleTask((updatestatus) =>
                                {
                                    updatestatus("Unloading assembly...");
                                    _distribInteraction.UnloadAssembly();
                                },() => "Assembly unloaded");
                        }, () => !_appState.AppBusy);
                }

                return _unloadAssemblyCommand;
            }
        }
    }
}
