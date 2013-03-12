using Distrib.Processes;
using DistribApps.Core.Processes.Hosting;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProcessNode.Modules.HostModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class HostCreationUIWindowViewModel :
        ViewModelBase
    {
        public HostCreationUIWindowViewModel()
            : base(false)
        {
        }

        private IHostProvider _provider;

        public IHostProvider HostProvider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;
                PropChanged("HostProvider");
            }
        }

        private Func<UserControl, string> _valFunc;
        private Func<UserControl, IProcessHost> _creationAction;

        private UserControl _providerUI;
        public UserControl ProviderUI
        {
            get
            {
                if (_providerUI == null)
                {
                    _providerUI = _provider.CreateWithUI(out _valFunc, out _creationAction);
                }

                return _providerUI;
            }
        }

        public IProcessHost CreatedHost
        {
            get;
            private set;
        }

        public Action<bool?> DialogResultAction
        {
            private get;
            set;
        }

        private DelegateCommand _acceptCommand;
        public ICommand AcceptCommand
        {
            get
            {
                if (_acceptCommand == null)
                {
                    _acceptCommand = new DelegateCommand(() =>
                        {
                            if (_valFunc != null)
                            {
                                var valMsg = _valFunc(_providerUI);
                                if (!string.IsNullOrEmpty(valMsg))
                                {
                                    MessageBox.Show(valMsg, "Error creating process host",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                                    return;
                                }
                                else
                                {
                                    this.CreatedHost = _creationAction(_providerUI);
                                    this.DialogResultAction(true);
                                }
                            }
                        });
                }

                return _acceptCommand;
            }
        }
    }
}
