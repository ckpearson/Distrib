using ProcessRunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.ViewModels
{
    public sealed class AssemblyDetailsViewModel : ViewModelBase
    {
        private IPrimaryViewModel _primaryViewModel;
        public AssemblyDetailsViewModel(IPrimaryViewModel primaryViewModel)
        {
            _primaryViewModel = primaryViewModel;
        }

        public PluginAssemblyModel Assembly
        {
            get
            {
                return _primaryViewModel.CurrentPluginAssembly;
            }
        }
    }
}
