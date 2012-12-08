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
            Bind<Plugins.IPluginAssemblyFactory>().To<Plugins.PluginAssemblyFactory>();
            Bind<Plugins.IPluginAssembly>().To<Plugins.PluginAssembly>();

            Bind<Plugins.IPluginAssemblyManagerFactory>().To<Plugins.PluginAssemblyManagerFactory>();
            Bind<Plugins.IPluginAssemblyManager>().To<Plugins.PluginAssemblyManager>();
        }
    }
}
