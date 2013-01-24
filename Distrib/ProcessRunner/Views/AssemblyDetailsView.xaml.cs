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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProcessRunner.Views
{
    /// <summary>
    /// Interaction logic for AssemblyDetailsView.xaml
    /// </summary>
    public partial class AssemblyDetailsView : UserControl, IView
    {
        public AssemblyDetailsView()
        {
            InitializeComponent();
        }

        public void NavigatingTo()
        {
        }

        public void NavigatedTo()
        {
        }

        public void NavigatingFrom()
        {
        }

        public void NavigatedFrom()
        {
        }
    }
}
