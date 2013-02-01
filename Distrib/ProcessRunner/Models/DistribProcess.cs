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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropChange([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

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
    }
}
