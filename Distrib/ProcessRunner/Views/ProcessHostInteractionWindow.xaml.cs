using ProcessRunner.Models;
using System;
using System.Collections.Generic;
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

namespace ProcessRunner.Views
{
    /// <summary>
    /// Interaction logic for ProcessHostInteractionWindow.xaml
    /// </summary>
    public partial class ProcessHostInteractionWindow : Elysium.Controls.Window
    {
        public ProcessHostInteractionWindow(ViewModels.ProcessHostInteractionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
