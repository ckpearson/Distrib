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
using Distrib.Plugins;
using Distrib.Processes;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using ProcessRunner.Models;
using ProcessRunner.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProcessRunner.ViewModels
{
    [Export()]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class ProcessesInvestigatorViewModel : ViewModelBase
    {
        private IDistribInteractionService _distrib;
        private IAppStateService _appState;
        private IEventAggregator _eventAggregator;

        [ImportingConstructor()]
        public ProcessesInvestigatorViewModel(IAppStateService appState, IDistribInteractionService distrib, IEventAggregator eventAgg)
        {
            _appState = appState;
            _distrib = distrib;
            _eventAggregator = eventAgg;

            _eventAggregator.GetEvent<Events.PluginAssemblyStateChangeEvent>()
                .Subscribe(OnAssemblyStateChanged, ThreadOption.UIThread);
            ProcessHosts = new ObservableCollection<DistribProcessHost>();
        }

        private IReadOnlyList<DistribProcess> _usableProcesses;

        public IReadOnlyList<DistribProcess> UsableProcesses
        {
            get
            {
                if (_usableProcesses == null)
                {
                    if (_distrib.CurrentAssembly != null && _distrib.CurrentAssembly.Initialised)
                    {
                        _usableProcesses = _distrib.CurrentAssembly.Plugins.Where(p => p.RawDescriptor.OfPluginInterface<IProcess>() &&
                                        p.IsUsable).Select(p => new DistribProcess(p, (proc, callback) =>
                                            {
                                                _appState.PerformVisibleTask((statUpdate) =>
                                                    {
                                                        statUpdate(string.Format("Creating process host for process '{0}'", proc.Plugin.PluginName));
                                                        var host = _distrib.CreateProcessHost(proc);
                                                        // I am thoroughly aware that this is nasty and shouldn't be here at all
                                                        // but it works and it's only a tiny whoops (I'm sure there are worse in here)
                                                        Application.Current.Dispatcher.BeginInvoke(new Action<IProcessHost>((h) =>
                                                            {
                                                                ProcessHosts.Add(callback(h));
                                                            }), host);
                                                    }, () =>
                                                    {

                                                        return "Created process host";
                                                    });
                                            }, (host) =>
                                            {
                                                Application.Current.Dispatcher.BeginInvoke(new Action<DistribProcessHost>((h) =>
                                                {
                                                    lock (_interactionWindows)
                                                    {
                                                        if (_interactionWindows.ContainsKey(h))
                                                        {
                                                            if (_interactionWindows[h] != null &&
                                                                _interactionWindows[h].IsVisible)
                                                            {
                                                                _interactionWindows[h].Close();
                                                            }

                                                            _interactionWindows.Remove(h);
                                                        }
                                                    }

                                                    ProcessHosts.Remove(h);
                                                }), host);
                                            },
                                            (host) =>
                                            {
                                                Application.Current.Dispatcher.BeginInvoke(new Action<DistribProcessHost>((h) =>
                                                    {
                                                        lock (_interactionWindows)
                                                        {
                                                            if (!_interactionWindows.ContainsKey(h))
                                                            {
                                                                _interactionWindows.Add(h, null);
                                                            }

                                                            if (_interactionWindows[h] != null)
                                                            {
                                                                _interactionWindows[h].Focus();
                                                            }
                                                            else
                                                            {

                                                                _interactionWindows[h] = new Views.ProcessHostInteractionWindow(
                                                                    new ProcessHostInteractionViewModel(
                                                                        _appState,
                                                                        _distrib,
                                                                        _eventAggregator,
                                                                        h));
                                                                _interactionWindows[h].Closing += (s, e) =>
                                                                    {
                                                                        if (h.IsProcessing)
                                                                        {
                                                                            e.Cancel = true;
                                                                        }
                                                                    };
                                                                _interactionWindows[h].Closed += (s, e) =>
                                                                    {
                                                                        _interactionWindows[h] = null;
                                                                    };
                                                                _interactionWindows[h].Show();
                                                                _interactionWindows[h].Activate();
                                                            }
                                                        }
                                                    }), host);
                                            })).ToList().AsReadOnly(); 
                    }
                }

                return _usableProcesses;
            }

            private set
            {
                _usableProcesses = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<DistribProcessHost, Views.ProcessHostInteractionWindow> _interactionWindows =
            new Dictionary<DistribProcessHost, Views.ProcessHostInteractionWindow>();

        public ObservableCollection<DistribProcessHost> ProcessHosts
        {
            get;
            set;
        }

        private void OnAssemblyStateChanged(Events.PluginAssemblyStateChange state)
        {
            if (state == Events.PluginAssemblyStateChange.AssemblyInitialised)
            {
                this.UsableProcesses = null;
                ProcessHosts.Clear();
            }
            else if (state == Events.PluginAssemblyStateChange.AssemblyUninitialised)
            {
                foreach (var procHost in ProcessHosts)
                {
                    procHost.Uninitialise();
                }
                this.UsableProcesses = null;
                ProcessHosts.Clear();
            }
        }
    }
}
