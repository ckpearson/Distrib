using DistribApps.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.Events
{
    public sealed class ViewBecameActiveEvent
    {
        private readonly ViewModelBase _viewModel;

        public ViewBecameActiveEvent(ViewModelBase viewModel)
        {
            _viewModel = viewModel;
        }

        public ViewModelBase ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
    }
}
