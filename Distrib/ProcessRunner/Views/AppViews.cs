using ProcessRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Views
{
    public static class AppViews
    {
        private static IPrimaryViewModel _primaryViewModel;

        public static IPrimaryViewModel PrimaryViewModel
        {
            get
            {
                if (_primaryViewModel == null)
                {
                    _primaryViewModel = new PrimaryViewModel();
                }

                return _primaryViewModel;
            }
        }

        private static IView _startView;
        public static IView StartView
        {
            get
            {
                if (_startView == null)
                {
                    var sv = new StartView();
                    sv.DataContext = new StartViewModel(PrimaryViewModel);
                    _startView = sv;
                }

                return _startView;
            }
        }

        private static IView _assemblyDetailsView;
        public static IView AssemblyDetailsView
        {
            get
            {
                if (_assemblyDetailsView == null)
                {
                    var asdv = new AssemblyDetailsView();
                    asdv.DataContext = new AssemblyDetailsViewModel(PrimaryViewModel);
                    _assemblyDetailsView = asdv;
                }

                return _assemblyDetailsView;
            }
        }
    }
}
