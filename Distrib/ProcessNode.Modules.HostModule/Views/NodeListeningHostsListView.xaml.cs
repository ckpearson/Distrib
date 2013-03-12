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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcessNode.Modules.HostModule.Views
{
    /// <summary>
    /// Interaction logic for NodeListeningHostsListView.xaml
    /// </summary>
    [Export]
    public partial class NodeListeningHostsListView : UserControl
    {
        public NodeListeningHostsListView()
        {
            InitializeComponent();

            RegionManager.SetRegionManager(this, ServiceLocator.Current.GetInstance<IRegionManager>());
            this.DataContext = ServiceLocator.Current.GetInstance<ViewModels.NodeListeningHostsListViewModel>();
        }
    }
}
