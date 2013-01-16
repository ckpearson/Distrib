using ProcessTester.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Ninject;
using Distrib.Plugins;

namespace ProcessTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window, INotifyPropertyChanged
    {
        private MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            _mainViewModel.CurrentAssembly = new Model.PluginAssemblyModel(IOC.Kernel.Get<IPluginAssemblyFactory>()
            .CreatePluginAssemblyFromPath(@"C:\Users\Clint\Desktop\distrib plugins\TestLibrary.dll"));

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void propChange(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    class dummy : Distrib.Plugins.IPluginAssembly
    {

        public Distrib.Plugins.IPluginAssemblyInitialisationResult Initialise()
        {
            throw new NotImplementedException();
        }

        public void Unitialise()
        {
            throw new NotImplementedException();
        }

        public bool IsInitialised
        {
            get { throw new NotImplementedException(); }
        }

        public string AssemblyFilePath
        {
            get { throw new NotImplementedException(); }
        }

        public Distrib.Plugins.IPluginInstance CreatePluginInstance(Distrib.Plugins.IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
