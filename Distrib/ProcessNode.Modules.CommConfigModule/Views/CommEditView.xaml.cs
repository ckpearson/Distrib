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

namespace ProcessNode.Modules.CommConfigModule.Views
{
    /// <summary>
    /// Interaction logic for CommEditView.xaml
    /// </summary>
    [Export]
    public partial class CommEditView : UserControl
    {
        [ImportingConstructor]
        public CommEditView(ViewModels.CommsEditViewModel vm)
        {
            InitializeComponent();

            this.DataContext = vm;
        }
    }
}
