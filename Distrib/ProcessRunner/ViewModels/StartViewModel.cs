using Microsoft.Win32;
using ProcessRunner.Commands;
using ProcessRunner.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessRunner.ViewModels
{
    public sealed class StartViewModel : ViewModelBase
    {
        private readonly IPrimaryViewModel _primaryViewModel;

        public StartViewModel(IPrimaryViewModel primaryViewModel)
        {
            _primaryViewModel = primaryViewModel;
        }

        public bool PluginAssemblyLoaded
        {
            get
            {
                return _primaryViewModel.CurrentPluginAssembly != null;
            }
        }

        public bool PluginAssemblyInitialised
        {
            get
            {
                if (!PluginAssemblyLoaded)
                {
                    return false;
                }
                else
                {
                    return _primaryViewModel.CurrentPluginAssembly.IsInitialised;
                }
            }
        }

        private RelayCommand _initialiseAssemblyCommand;
        public ICommand InitialiseAssemblyCommand
        {
            get
            {
                if (_initialiseAssemblyCommand == null)
                {
                    _initialiseAssemblyCommand = new RelayCommand((p) =>
                        {
                            _primaryViewModel.DoAsBusy(() =>
                                {
                                    _primaryViewModel.InitAssembly();
                                },
                                () =>
                                {
                                    onPropChange("PluginAssemblyInitialised");
                                    _primaryViewModel.Navigate(AppViews.AssemblyDetailsView);
                                });
                        }, (p) =>
                            {
                                return PluginAssemblyLoaded && !_primaryViewModel.IsBusy;
                            });
                }

                return _initialiseAssemblyCommand;
            }
        }

        private RelayCommand _chooseAnotherCommand;
        public ICommand ChooseAnotherAssemblyCommand
        {
            get
            {
                if (_chooseAnotherCommand == null)
                {
                    _chooseAnotherCommand = new RelayCommand((p) =>
                        {
                            _primaryViewModel.DoAsBusy(() =>
                                {
                                    _primaryViewModel.UnloadAssembly();
                                }, () =>
                                {
                                    onPropChange("PluginAssemblyLoaded");
                                });
                        }, (p) =>
                            {
                                return PluginAssemblyLoaded && !_primaryViewModel.IsBusy;
                            });
                }

                return _chooseAnotherCommand;
            }
        }

        private RelayCommand _loadAssemblyCommand;
        public ICommand LoadAssemblyCommand
        {
            get
            {
                if (_loadAssemblyCommand == null)
                {
                    _loadAssemblyCommand = new RelayCommand((p) =>
                        {
                            _primaryViewModel.DoAsBusy(() =>
                                {
                                    var ofd = new OpenFileDialog();
                                    ofd.Title = "Select assembly";
                                    ofd.Filter = "Assemblies|*.dll";
                                    ofd.Multiselect = false;
                                    if (ofd.ShowDialog().Value == true)
                                    {
                                        _primaryViewModel.LoadAssembly(ofd.FileName);
                                    }
                                }, () =>
                                {
                                    onPropChange("PluginAssemblyLoaded");
                                });
                        });
                }

                return _loadAssemblyCommand;
            }
        }
    }
}
