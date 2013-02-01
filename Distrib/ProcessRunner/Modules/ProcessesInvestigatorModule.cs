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
    [ModuleExport(typeof(ProcessesInvestigatorModule))]
    public sealed class ProcessesInvestigatorModule : IModule
    {
        private IRegionManager _regionManager;

        [ImportingConstructor()]
        public ProcessesInvestigatorModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ProcessesInvestigatorRegion", typeof(Views.ProcessesInvestigatorView));
        }
    }
}
