using Microsoft.Win32;
using ProcessTester.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using Distrib.Plugins;

namespace ProcessTester.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        private PluginAssemblyModel _assembly;
        public PluginAssemblyModel CurrentAssembly
        {
            get { return _assembly; }
            set
            {
                _assembly = value;
                PropChange();
            }
        }

        private RelayCommand _chooseAssemblyCommand;
        public ICommand ChooseAssemblyCommand
        {
            get
            {
                if (_chooseAssemblyCommand == null)
                {
                    _chooseAssemblyCommand = new RelayCommand((param) =>
                        {
                            var fod = new OpenFileDialog();
                            fod.Title = "Select an assembly";
                            fod.Multiselect = false;
                            fod.Filter = "Assemblies|*.dll";
                            if (fod.ShowDialog().Value)
                            {
                                var path = fod.FileName;
                                // Try to load
                                CurrentAssembly = new PluginAssemblyModel(IOC.Kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(path));
                            }
                        });
                }

                return _chooseAssemblyCommand;
            }
        }

        private RelayCommand _unloadAssemblyCommand;
        public ICommand UnloadAssemblyCommand
        {
            get
            {
                if (_unloadAssemblyCommand == null)
                {
                    _unloadAssemblyCommand = new RelayCommand((param) =>
                    {
                        if (CurrentAssembly != null)
                        {
                            if (CurrentAssembly.Initialised)
                            {
                                CurrentAssembly.ToggleInitCommand.Execute(null);
                            }

                            if (CurrentAssembly.ProcessHost != null)
                            {
                                if (CurrentAssembly.ProcessHost.IsInitialised)
                                {
                                    CurrentAssembly.ProcessHost.Unitialise();
                                }

                                CurrentAssembly.ProcessHost = null;
                            }

                            CurrentAssembly = null;
                        }
                    }, (arg) =>
                    {
                        return (CurrentAssembly != null);
                    });
                }

                return _unloadAssemblyCommand;
            }
        }
    }
}
