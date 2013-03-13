using Distrib.Processes;
using DistribApps.Core.Processes.Hosting;
using DistribApps.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.HostProviders.PluginPowered.Providers
{
    [Export(typeof(IHostProvider))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    class PluginPoweredHostProvider :
        IHostProvider
    {
        public string Name
        {
            get { return "Plugin-powered host provider"; }
        }

        public string Description
        {
            get { return "Creates a plugin-powered process host from a distrib plugin assembly"; }
        }

        public string UIFromText
        {
            get { return "From Process Plugin"; }
        }

        public bool HasUI
        {
            get { return true; }
        }

        [Import]
        private IDistribAccessService _distrib;

        public System.Windows.Controls.UserControl CreateWithUI(out Func<System.Windows.Controls.UserControl, string> validationFunc, out Func<System.Windows.Controls.UserControl, Distrib.Processes.IProcessHost> creationAction)
        {
            var uc = new Views.ProviderView();

            uc.DataContext = new ViewModels.ProviderViewModel(_distrib);

            validationFunc = (cont) =>
                {
                    var vm = ((Views.ProviderView)cont).DataContext as ViewModels.ProviderViewModel;

                    if (!vm.AssemblySelected)
                    {
                        return "A plugin assembly must be chosen";
                    }

                    if (vm.SelectedPlugin == null)
                    {
                        return "A plugin must be selected";
                    }

                    return null;
                };

            creationAction = (cont) =>
                {
                    var vm = ((Views.ProviderView)cont).DataContext as ViewModels.ProviderViewModel;
                    return _distrib.DistribIOC.Get<IProcessHostFactory>()
                        .CreateHostFromPlugin(vm.SelectedPlugin);
                };

            return uc;
        }

        public Distrib.Processes.IProcessHost CreateWithoutUI()
        {
            throw new NotImplementedException("Host provider doesn't support non-ui based creation");
        }
    }
}
