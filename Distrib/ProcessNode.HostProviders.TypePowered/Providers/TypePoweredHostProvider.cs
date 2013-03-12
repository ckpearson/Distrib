using Distrib.Processes;
using DistribApps.Core.Processes.Hosting;
using DistribApps.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProcessNode.HostProviders.TypePowered.Providers
{
    [Export(typeof(IHostProvider))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class TypePoweredHostProvider
        : IHostProvider
    {
        public string Name
        {
            get { return "Type-powered host provider"; }
        }

        public string Description
        {
            get { return "Creates a type-powered process host from an assembly"; }
        }

        public string UIFromText
        {
            get { return "From Process Type"; }
        }

        public bool HasUI
        {
            get { return true; }
        }

        [Import]
        private IDistribAccessService _distrib;

        public System.Windows.Controls.UserControl CreateWithUI(out Func<UserControl, string> validationFunc,
            out Func<UserControl, IProcessHost> creationAction)
        {
            var uc = new Views.ProviderView();
            uc.DataContext = new ViewModels.ProviderViewModel();

            validationFunc = (cont) =>
                {
                    var vm = (ViewModels.ProviderViewModel)((Views.ProviderView)cont).DataContext;

                    if (!vm.AssemblySelected)
                    {
                        return "An assembly must be selected and a type chosen";
                    }

                    if (vm.SelectedType == null)
                    {
                        return "A process type must be chosen";
                    }

                    return null;
                };

            creationAction = (cont) =>
            {
                var vm = (ViewModels.ProviderViewModel)((Views.ProviderView)cont).DataContext;

                return _distrib.DistribIOC.Get<IProcessHostFactory>()
                    .CreateHostFromType(vm.SelectedType.ActualType);
            };

            return uc;
        }

        public Distrib.Processes.IProcessHost CreateWithoutUI()
        {
            throw new NotImplementedException("Host provider doesn't support non-ui based creation");
        }
    }
}
