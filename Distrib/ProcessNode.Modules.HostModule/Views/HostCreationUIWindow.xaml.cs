using DistribApps.Core.Processes.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace ProcessNode.Modules.HostModule.Views
{
    /// <summary>
    /// Interaction logic for HostCreationUIWindow.xaml
    /// </summary>
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class HostCreationUIWindow : Elysium.Controls.Window
    {
        [ImportingConstructor]
        public HostCreationUIWindow(ViewModels.HostCreationUIWindowViewModel vm)
        {
            InitializeComponent();

            vm.DialogResultAction = (res) =>
                {
                    this.DialogResult = res;
                };
            this.DataContext = vm;
        }

        public ViewModels.HostCreationUIWindowViewModel VM
        {
            get
            {
                return (ViewModels.HostCreationUIWindowViewModel)this.DataContext;
            }
        }
    }
}
