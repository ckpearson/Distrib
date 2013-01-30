using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Modules
{
    [ModuleExport(typeof(BusyIndicatorModule))]
    public sealed class BusyIndicatorModule : IModule
    {
        private IRegionManager _regionManager;

        [ImportingConstructor()]
        public BusyIndicatorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("BusyIndicatorRegion", typeof(Views.BusyIndicatorView));
        }
    }
}
