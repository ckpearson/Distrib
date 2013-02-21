/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class PluginsIOCRegistrar : IOCRegistrar
    {
        public override void PerformBindings()
        {
            BindSingleton<Plugins.IPluginAssemblyFactory, Plugins.PluginAssemblyFactory>();
            Bind<Plugins.IPluginAssembly, Plugins.PluginAssembly>();

            BindSingleton<Plugins.IPluginAssemblyManagerFactory, Plugins.PluginAssemblyManagerFactory>();
            Bind<Plugins.IPluginAssemblyManager, Plugins.PluginAssemblyManager>();

            BindSingleton<Plugins.IPluginDescriptorFactory, Plugins.PluginDescriptorFactory>();
            Bind<Plugins.IPluginDescriptor, Plugins.PluginDescriptor>();

            BindSingleton<Plugins.IPluginMetadataFactory, Plugins.PluginMetadataFactory>();
            Bind<Plugins.IPluginMetadata, Plugins.PluginMetadata>();

            BindSingleton<Plugins.IPluginMetadataBundleFactory, Plugins.PluginMetadataBundleFactory>();
            Bind<Plugins.IPluginMetadataBundle, Plugins.PluginMetadataBundle>();

            BindSingleton<Plugins.IPluginBootstrapServiceFactory, Plugins.PluginBootstrapServiceFactory>();
            Bind<Plugins.IPluginBootstrapService, Plugins.PluginBootstrapService>();

            BindSingleton<Plugins.IPluginCoreUsabilityCheckServiceFactory, Plugins.PluginCoreUsabilityCheckServiceFactory>();
            Bind<Plugins.IPluginCoreUsabilityCheckService, Plugins.PluginCoreUsabilityCheckService>();

            BindSingleton<Plugins.IPluginControllerValidationServiceFactory, Plugins.PluginControllerValidationServiceFactory>();
            Bind<Plugins.IPluginControllerValidationService, Plugins.PluginControllerValidationService>();

            BindSingleton<Plugins.IPluginMetadataBundleCheckServiceFactory, Plugins.PluginMetadataBundleCheckServiceFactory>();
            Bind<Plugins.IPluginMetadataBundleCheckService, Plugins.PluginMetadataBundleCheckService>();

            BindSingleton<Plugins.IPluginControllerFactory, Plugins.PluginControllerFactory>();
            Bind<Plugins.IPluginController, Plugins.StandardPluginController>();

            BindSingleton<Plugins.IPluginInteractionLinkFactory, Plugins.PluginInteractionLinkFactory>();
            Bind<Plugins.IPluginInteractionLink, Plugins.StandardPluginInteractionLink>();

            BindSingleton<Plugins.IPluginAssemblyInitialisationResultFactory, Plugins.PluginAssemblyInitialisationResultFactory>();
            Bind<Plugins.IPluginAssemblyInitialisationResult, Plugins.PluginAssemblyInitialisationResult>();

            BindSingleton<Plugins.IPluginInstanceFactory, Plugins.PluginInstanceFactory>();
            Bind<Plugins.IPluginInstance, Plugins.PluginInstance>();
        }
    }
}
