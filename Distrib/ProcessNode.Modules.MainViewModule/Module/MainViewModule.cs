using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using ProcessNode.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Modules.MainViewModule.Module
{
    [ModuleExport(typeof(IModule))]
    public sealed class MainViewModule : IModule
    {
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public MainViewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(AppRegions.MainRegion, typeof(Views.MainView));
        }
    }
}
