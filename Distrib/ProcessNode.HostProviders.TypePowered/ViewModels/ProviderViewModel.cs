using Distrib.Processes;
using DistribApps.Core.Services;
using DistribApps.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessNode.HostProviders.TypePowered.ViewModels
{
    public sealed class ProviderViewModel :
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ProviderViewModel()
        {

        }

        public bool AssemblySelected
        {
            get
            {
                return _assembly != null;
            }
        }

        private Assembly _assembly;

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
                    return new DirectoryInfo(Path.GetDirectoryName(_assembly.Location))
                    .Name + "\\" + new FileInfo(_assembly.Location).Name + new FileInfo(_assembly.Location).Extension;
                }
            }
        }

        private IEnumerable<ProcessType> _processTypes;

        public IEnumerable<ProcessType> ProcessTypes
        {
            get
            {
                if (_processTypes == null)
                {
                    _processTypes = _assembly.GetTypes()
                        .Where(t => t.GetInterface(typeof(IProcess).FullName) != null
                            && Attribute.IsDefined(t, typeof(Distrib.Processes.TypePowered.ProcessMetadataAttribute)))
                        .Select(t => new ProcessType(t));
                }

                return _processTypes;
            }
        }

        private ProcessType _selectedType;
        public ProcessType SelectedType
        {
            get
            {
                return _selectedType;
            }

            set
            {
                _selectedType = value;
                propChange();
            }
        }

        public Assembly Assembly
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
                propChange("ProcessTypes");
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
                                this.Assembly = Assembly.LoadFrom(asmPath);
                            }
                        });
                }

                return _chooseAssemblyCommand;
            }
        }

        private void propChange([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public sealed class ProcessType : INotifyPropertyChanged
    {
        private readonly Type _type;

        private IProcessMetadata _metadata;

        public ProcessType(Type type)
        {
            _type = type;
        }

        public string TypeName
        {
            get { return _type.FullName; }
        }

        public Type ActualType
        {
            get { return _type; }
        }

        public bool MoreInfoAvailable
        {
            get
            {
                return  _metadata != null;
            }
        }

        public IProcessMetadata Metadata
        {
            get
            {
                return _metadata;
            }
        }

        private DelegateCommand _moreInfoCommand;
        public ICommand MoreInformationCommand
        {
            get
            {
                if (_moreInfoCommand == null)
                {
                    _moreInfoCommand = new DelegateCommand(GetMoreInfo);
                }

                return _moreInfoCommand;
            }
        }

        private void GetMoreInfo()
        {
            try
            {
                if (_metadata == null)
                {
                    var host = ServiceLocator.Current.GetInstance<IDistribAccessService>()
                        .DistribIOC.Get<IProcessHostFactory>()
                        .CreateHostFromType(_type);

                    host.Initialise();

                    _metadata = host.Metadata;

                    host.Unitialise();
                    host = null;

                    prop("Metadata");
                    prop("MoreInfoAvailable");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get more info", ex);
            }
        }

        private void prop([CallerMemberName] string property = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
