/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
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
