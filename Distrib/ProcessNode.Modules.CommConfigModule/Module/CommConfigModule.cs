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

namespace ProcessNode.Modules.CommConfigModule.Module
{
    [ModuleExport(typeof(CommConfigModule))]
    public sealed class CommConfigModule :
        IModule
    {
        private readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public CommConfigModule(
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(AppRegions.CommsRegion, typeof(Views.CommConfigView));
            _regionManager.RegisterViewWithRegion(CommsRegionNames.CommsEditRegion, typeof(Views.CommEditView));
            _regionManager.RegisterViewWithRegion(CommsRegionNames.CommsInteractRegion, typeof(Views.CommInteractView));
        }
    }
}
