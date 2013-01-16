using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Microsoft.Win32;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcessTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window, INotifyPropertyChanged
    {
        private readonly IKernel _kernel;

        public MainWindow()
        {
            _kernel = new StandardKernel(typeof(PluginsNinjectModule).Assembly.GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Equals(typeof(NinjectModule)))
                    .Select(t => Activator.CreateInstance(t) as INinjectModule)
                    .ToArray());

            InitializeComponent();

            this.DataContext = this;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_assembly != null)
            {
                _unloadCurrentAssembly();
            }
        }

        public string CurrentAssemblyPath
        {
            get
            {
                if (_assembly == null)
                    return "No assembly chosen";

                return _assembly.AssemblyFilePath;
            }
        }

        public ObservableCollection<IPluginDescriptor> Processes
        {
            get;
            set;
        }

        private void btnChoose_Click_1(object sender, RoutedEventArgs e)
        {
            var fod = new OpenFileDialog();
            fod.Title = "Select an assembly";
            fod.Filter = "Assembly|*.dll";
            fod.Multiselect = false;

            if (fod.ShowDialog().Value)
            {
                var asmPath = fod.FileName;
                _loadAssembly(asmPath);
            }
        }

        private IPluginAssembly _assembly;

        private void _loadAssembly(string asmPath)
        {
            if (_assembly != null)
            {
                _unloadCurrentAssembly();
            }

            _assembly = _kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(asmPath);
            prop("CurrentAssemblyPath");
            var res = _assembly.Initialise();
            if (!res.HasUsablePlugins)
            {
                MessageBox.Show("No usable plugins found in assembly");
                _unloadCurrentAssembly();
            }

            var procs = res.UsablePlugins.Where(p => p.Metadata.InterfaceType.Equals(typeof(IProcess)))
                .ToList();

            if (procs == null || procs.Count == 0)
            {
                MessageBox.Show("No process plugins present in assembly");
                _unloadCurrentAssembly();
            }

            if (this.Processes == null)
                this.Processes = new ObservableCollection<IPluginDescriptor>();
            else
                this.Processes.Clear();

            foreach (var procdesc in procs)
            {
                this.Processes.Add(procdesc);
                prop("Processes");
            }
        }

        private void _unloadCurrentAssembly()
        {
            if (_assembly.IsInitialised)
            {
                _assembly.Unitialise();
            }
            _assembly = null;
            prop("CurrentAssemblyPath");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void prop(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
