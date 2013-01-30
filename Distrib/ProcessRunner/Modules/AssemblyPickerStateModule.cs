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
    [ModuleExport(typeof(AssemblyPickerStateModule))]
    public sealed class AssemblyPickerStateModule : IModule
    {
        private IRegionManager _regionManager;

        [ImportingConstructor()]
        public AssemblyPickerStateModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("AssemblyStateRegion", typeof(Views.AssemblyPickerStateView));
        }
    }
}
