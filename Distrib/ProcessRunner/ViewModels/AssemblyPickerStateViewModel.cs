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
