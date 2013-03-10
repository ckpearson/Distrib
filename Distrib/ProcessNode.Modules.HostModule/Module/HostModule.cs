using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using ProcessNode.Shared;
using ProcessNode.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Modules.HostModule.Module
{
    [ModuleExport(typeof(HostModule))]
    public sealed class HostModule :
        IModule
    {
        private readonly IRegionManager _regionManager;
        [ImportingConstructor]
        public HostModule(IRegionManager manager)
        {
            _regionManager = manager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(AppRegions.ContentRegion, () => ServiceLocator.Current.GetInstance<Views.HostsListView>());
        }
    }
}
