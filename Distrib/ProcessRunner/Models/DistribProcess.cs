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
