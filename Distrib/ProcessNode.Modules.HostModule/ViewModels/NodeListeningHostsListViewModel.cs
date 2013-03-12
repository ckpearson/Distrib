using Distrib.Processes;
using DistribApps.Core.Events;
using DistribApps.Core.Processes.Hosting;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using ProcessNode.Modules.HostModule.Views;
using ProcessNode.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.Modules.HostModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class NodeListeningHostsListViewModel :
        ViewModelBase
    {
        private readonly INewEventAggregator _eventAgg;
        private readonly INodeHostingService _hosting;

        [ImportingConstructor]
        public NodeListeningHostsListViewModel(
            INewEventAggregator eventAgg,
            INodeHostingService hosting)
            : base(true)
        {
            _eventAgg = eventAgg;
            _hosting = hosting;

            base.IsActiveChanged += NodeListeningHostsListViewModel_IsActiveChanged;
        }

        void NodeListeningHostsListViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            if (base.IsActive)
            {
                _eventAgg.Send(new ViewBecameActiveEvent(this));
                PropChanged("Node");
                PropChanged("HostProviders");
            }
        }

        // ViewModel

        public IManagedProcessNode Node
        {
            get { return _hosting.Node; }
        }

        public IEnumerable<IHostProvider> HostProviders
        {
            get { return _hosting.HostProviders; }
        }

        private DelegateCommand<IHostProvider> _createFromProviderCommand;
        public ICommand CreateFromHostProviderCommand
        {
            get
            {
                if (_createFromProviderCommand == null)
                {
                    _createFromProviderCommand = new DelegateCommand<IHostProvider>((provider) =>
                        {
                            IProcessHost host;

                            if (provider.HasUI)
                            {
                                var window = ServiceLocator.Current.GetInstance<Views.HostCreationUIWindow>();
                                var vm = window.VM;

                                vm.HostProvider = provider;
                                var res = window.ShowDialog();
                                if (!res.HasValue || !res.Value)
                                {
                                    return;
                                }

                                host = vm.CreatedHost;
                            }
                            else
                            {
                                host = provider.CreateWithoutUI();
                            }

                            _hosting.Node.AddHost(new ManagedProcessHost(host));

                        });
                }

                return _createFromProviderCommand;
            }
        }
    }
}
