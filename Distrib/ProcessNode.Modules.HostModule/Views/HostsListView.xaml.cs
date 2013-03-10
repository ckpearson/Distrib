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
    /// Interaction logic for HostsListView.xaml
    /// </summary>
    [Export]
    public partial class HostsListView : UserControl
    {
        [ImportingConstructor]
        public HostsListView(ViewModels.HostsListViewViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;
        }
    }
}
