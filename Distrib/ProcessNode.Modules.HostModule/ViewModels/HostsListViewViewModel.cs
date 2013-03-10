using DistribApps.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Modules.HostModule.ViewModels
{
    [Export]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public sealed class HostsListViewViewModel : 
        ViewModelBase
    {
        public HostsListViewViewModel()
            : base(true)
        {
            base.IsActiveChanged += HostsListViewViewModel_IsActiveChanged;
        }

        void HostsListViewViewModel_IsActiveChanged(object sender, EventArgs e)
        {
            var b = base.IsActive;
        }

        protected override void OnNavigatedTo(Microsoft.Practices.Prism.Regions.NavigationContext context)
        {
        }
    }
}
