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
                                        try
                                        {
                                            _distribInteraction.LoadAssembly(ofd.FileName);
                                            updateStatus("Assembly loaded");
                                        }
                                        catch (Exception ex)
                                        {
                                            var baseExcep = ex;
                                            updateStatus(string.Format("An error occurred: '{0}' ({1}) \n{2}",
                                                baseExcep.Message,
                                                baseExcep.Source,
                                                baseExcep.InnerException != null ?
                                                " - \"" + baseExcep.GetBaseException().Message + "\" (" +
                                                    baseExcep.GetBaseException().Source + ")" : ""));
                                        }
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
