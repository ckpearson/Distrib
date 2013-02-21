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
using Distrib.Processes;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.Models
{
    public sealed class DistribProcess : INotifyPropertyChanged
    {
        private DistribPlugin _processPlugin;
        private Action<DistribProcess, Func<IProcessHost, DistribProcessHost>> _spawnHostFunc;
        private Action<DistribProcessHost> _hostTerminatedAction;
        private Action<DistribProcessHost> _interactWithHostAction;

        public DistribProcess(DistribPlugin plugin, Action<DistribProcess, Func<IProcessHost, DistribProcessHost>> spawnHostFunc,
            Action<DistribProcessHost> hostTerminatedAction,
            Action<DistribProcessHost> hostInteractionAction)
        {
            _processPlugin = plugin;
            _spawnHostFunc = spawnHostFunc;
            _hostTerminatedAction = hostTerminatedAction;
            _interactWithHostAction = hostInteractionAction;

            ProcessHosts = new ObservableCollection<DistribProcessHost>();
        }

        public DistribPlugin Plugin
        {
            get { return _processPlugin; }
        }

        public bool HasHosts
        {
            get { return ProcessHosts != null && ProcessHosts.Count > 0;}
        }

        public ObservableCollection<DistribProcessHost> ProcessHosts
        {
            get;
            private set;
        }

        private DelegateCommand _spawnHostCommand;
        public ICommand SpawnHostCommand
        {
            get
            {
                if (_spawnHostCommand == null)
                {
                    _spawnHostCommand = new DelegateCommand(() =>
                        {
                            _spawnHostFunc(this, (proc) =>
                                {
                                    var host = new DistribProcessHost(this, proc);
                                    ProcessHosts.Add(host);
                                    OnPropChange("HasHosts");
                                    return host;
                                });
                        });
                }

                return _spawnHostCommand;
            }
        }

        private DelegateCommand _killAllHostsCommand;
        public ICommand KillAllHostsCommand
        {
            get
            {
                if (_killAllHostsCommand == null)
                {
                    _killAllHostsCommand = new DelegateCommand(() =>
                        {
                            var hosts = ProcessHosts.ToList().AsReadOnly();
                            foreach (var h in hosts)
                            {
                                h.KillCommand.Execute(null);
                            }
                        });
                }

                return _killAllHostsCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChange([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Called to kill a given host
        /// </summary>
        /// <param name="host"></param>
        public void KillHost(DistribProcessHost host)
        {
            host.Uninitialise();
            ProcessHosts.Remove(host);
            _hostTerminatedAction(host);
        }

        internal void InteractWithHost(DistribProcessHost host)
        {
            _interactWithHostAction(host);
        }

        /// <summary>
        /// Called to inform the process that a host is uninitialising
        /// </summary>
        /// <param name="host"></param>
        public void HostUninitialising(DistribProcessHost host)
        {
            ProcessHosts.Remove(host);
            _hostTerminatedAction(host);
        }
    }
}
