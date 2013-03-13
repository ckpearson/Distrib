using Distrib.Plugins;
using Distrib.Processes;
using DistribApps.Core.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.HostProviders.PluginPowered.ViewModels
{
    public sealed class ProviderViewModel :
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void propChange([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public ProviderViewModel(IDistribAccessService distrib)
        {
            _distrib = distrib;
        }

        private IDistribAccessService _distrib;

        public bool AssemblySelected
        {
            get { return _assembly != null; }
        }

        private IPluginAssembly _assembly;
        private IPluginAssemblyInitialisationResult _initResult;

        public string AssemblyPath
        {
            get
            {
                if (_assembly == null)
                {
                    return null;
                }
                else
                {
                    return new DirectoryInfo(Path.GetDirectoryName(_assembly.AssemblyFilePath))
                    .Name + "\\" + new FileInfo(_assembly.AssemblyFilePath).Name + new FileInfo(_assembly.AssemblyFilePath).Extension;
                }
            }
        }

        public IPluginAssemblyInitialisationResult InitResult
        {
            get
            {
                return _initResult;
            }

            set
            {
                _initResult = value;
                propChange();
                propChange("HasUsablePlugins");
                propChange("UsableProcessPlugins");
            }
        }

        public bool HasUsablePlugins
        {
            get
            {
                return _initResult != null ? _initResult.HasUsablePlugins : false;
            }
        }

        public IEnumerable<IPluginDescriptor> UsableProcessPlugins
        {
            get
            {
                return _initResult != null ? _initResult.UsablePlugins
                    .Where(pl => pl.OfPluginInterface<IProcess>()) : null;
            }
        }

        private IPluginDescriptor _selectedPlugin;
        public IPluginDescriptor SelectedPlugin
        {
            get
            {
                return _selectedPlugin;
            }

            set
            {
                _selectedPlugin = value;
                propChange();
            }
        }

        public IPluginAssembly Assembly
        {
            get
            {
                return _assembly;
            }

            set
            {
                _assembly = value;
                propChange();
                propChange("AssemblySelected");
                propChange("AssemblyPath");
            }
        }

        private DelegateCommand _chooseAssemblyCommand;
        public ICommand ChooseAssemblyCommand
        {
            get
            {
                if (_chooseAssemblyCommand == null)
                {
                    _chooseAssemblyCommand = new DelegateCommand(() =>
                        {
                            var ofd = new OpenFileDialog();
                            ofd.Title = "Select an assembly";
                            ofd.Multiselect = false;
                            ofd.Filter = "Assemblies|*.dll";

                            if (ofd.ShowDialog().Value)
                            {
                                var asmPath = ofd.FileName;
                                this.Assembly = _distrib.DistribIOC.Get<IPluginAssemblyFactory>()
                                    .CreatePluginAssemblyFromPath(asmPath);
                                this.InitResult = this.Assembly.Initialise();
                                this.Assembly.Unitialise();
                            }
                        });
                }

                return _chooseAssemblyCommand;
            }
        }
    }
}
