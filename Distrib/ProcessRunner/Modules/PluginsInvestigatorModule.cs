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
    [ModuleExport(typeof(PluginsInvestigatorModule))]
    public sealed class PluginsInvestigatorModule : IModule
    {
        private IRegionManager _regionManager;

        [ImportingConstructor()]
        public PluginsInvestigatorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("PluginsInvestigatorRegion", typeof(Views.PluginsInvestigatorView));
        }
    }
}
