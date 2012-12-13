using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class PluginsNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Plugins.IPluginAssemblyFactory>().To<Plugins.PluginAssemblyFactory>().InSingletonScope();
            Bind<Plugins.IPluginAssembly>().To<Plugins.PluginAssembly>();

            Bind<Plugins.IPluginAssemblyManagerFactory>().To<Plugins.PluginAssemblyManagerFactory>().InSingletonScope();
            Bind<Plugins.IPluginAssemblyManager>().To<Plugins.PluginAssemblyManager>();


            Bind<Plugins.IPluginDescriptorFactory>().To<Plugins.PluginDescriptorFactory>().InSingletonScope();
            Bind<Plugins.IPluginDescriptor>().To<Plugins.PluginDescriptor>();

            Bind<Plugins.IPluginMetadataFactory>().To<Plugins.PluginMetadataFactory>().InSingletonScope();
            Bind<Plugins.IPluginMetadata>().To<Plugins.PluginMetadata>();

            Bind<Plugins.IPluginMetadataBundleFactory>().To<Plugins.PluginMetadataBundleFactory>().InSingletonScope();
            Bind<Plugins.IPluginMetadataBundle>().To<Plugins.PluginMetadataBundle>();

            Bind<Plugins.IPluginBootstrapServiceFactory>().To<Plugins.PluginBootstrapServiceFactory>().InSingletonScope();
            Bind<Plugins.IPluginBootstrapService>().To<Plugins.PluginBootstrapService>().InSingletonScope();

            Bind<Plugins.IPluginCoreUsabilityCheckServiceFactory>().To<Plugins.PluginCoreUsabilityCheckServiceFactory>().InSingletonScope();
            Bind<Plugins.IPluginCoreUsabilityCheckService>().To<Plugins.PluginCoreUsabilityCheckService>().InSingletonScope();

            Bind<Plugins.IPluginControllerValidationServiceFactory>().To<Plugins.PluginControllerValidationServiceFactory>().InSingletonScope();
            Bind<Plugins.IPluginControllerValidationService>().To<Plugins.PluginControllerValidationService>().InSingletonScope();

            Bind<Plugins.IPluginMetadataBundleCheckServiceFactory>().To<Plugins.PluginMetadataBundleCheckServiceFactory>().InSingletonScope();
            Bind<Plugins.IPluginMetadataBundleCheckService>().To<Plugins.PluginMetadataBundleCheckService>().InSingletonScope();

            Bind<Plugins.IPluginControllerFactory>().To<Plugins.PluginControllerFactory>().InSingletonScope();
            Bind<Plugins.IPluginController>().To<Plugins.StandardPluginController>();

            Bind<Plugins.IPluginAssemblyInitialisationResultFactory>().To<Plugins.PluginAssemblyInitialisationResultFactory>()
                .InSingletonScope();

            Bind<Plugins.IPluginAssemblyInitialisationResult>().To<Plugins.PluginAssemblyInitialisationResult>();

            Bind<Plugins.IPluginInstanceFactory>().To<Plugins.PluginInstanceFactory>().InSingletonScope();
            Bind<Plugins.IPluginInstance>().To<Plugins.PluginInstance>();
        }
    }
}
