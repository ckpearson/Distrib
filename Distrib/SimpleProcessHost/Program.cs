using Distrib.Plugins;
using Distrib.Processes;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleProcessHost
{
    class Program
    {
        private readonly string pluginsDir =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "distrib plugins");

        private readonly ConsoleWriter Output = new ConsoleWriter();

        static void Main(string[] args)
        {
            new Program().Run();

            Console.WriteLine();
            Console.WriteLine("<ENTER> to terminate");
            Console.ReadLine();
        }

        private void Run()
        {
            // Create Ninject kernel loading all the modules in the Distrib assembly
            var kernel = new StandardKernel(
                typeof(Distrib.IOC.PluginsNinjectModule).Assembly.GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.Equals(typeof(Ninject.Modules.NinjectModule)))
                .Select(t => Activator.CreateInstance(t) as Ninject.Modules.NinjectModule)
                .ToArray());

            Console.WriteLine("Looking for plugins in: '{0}'", pluginsDir);

            foreach (var pluginDll in Directory.EnumerateFiles(pluginsDir, "*.dll"))
            {
                Output.Indent++;
                Console.WriteLine("Investigating assembly: {0}", Path.GetFileName(pluginDll));

                var pluginAssembly = kernel.Get<IPluginAssemblyFactory>()
                    .CreatePluginAssemblyFromPath(pluginDll);

                var assemblyRes = pluginAssembly.Initialise();

                if (!assemblyRes.HasUsablePlugins)
                {
                    Output.Indent++;
                    Console.WriteLine("No usable plugins found in assembly: {0}", Path.GetFileName(pluginDll));
                    Output.Indent--;
                }
                else
                {
                    if (!assemblyRes.UsablePlugins.Any(p => p.Metadata.InterfaceType.Equals(typeof(IDistribProcess))))
                    {
                        Output.Indent++;
                        Console.WriteLine("No processes found in assembly: {0}", Path.GetFileName(pluginDll));
                        Output.Indent--;
                    }
                }
            }
        }
    }
}
