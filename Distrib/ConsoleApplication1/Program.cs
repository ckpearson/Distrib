using Distrib.Plugins;
using Distrib.Plugins.Description;
using Distrib.Plugins.Discovery;
using Distrib.Processes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        private string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            var pd = new PluginDetails(null, null);

            foreach (var pluginDll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var pluginAssembly = DistribPluginAssembly.CreateForAssembly(pluginDll);
                var result = pluginAssembly.Initialise();

                if (result.HasUsablePlugins)
                {
                    var firstProcPlugin = result.UsablePlugins.DefaultIfEmpty(null)
                        .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IDistribProcess)));

                    if (firstProcPlugin == null)
                    {
                        throw new InvalidOperationException("No usable process plugin exists");
                    }

                    var inst = pluginAssembly.CreatePluginInstance(firstProcPlugin);

                    var proc = inst.GetInstance<IDistribProcess>();

                }

               // File.Delete(pluginDll);

                pluginAssembly.Uninitialise();
            }
        }

        public void RunPluginList()
        {
            Console.WriteLine("Distrib Plugin Discovery:");
            DashedLine();

            Console.WriteLine("Looking for plugins in directory: \"{0}\"", dir);

            foreach (var pluginDll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                Console.WriteLine();
                Console.WriteLine("\tAssembly: \"{0}\"", Path.GetFileName(pluginDll));
                var pluginAssembly = DistribPluginAssembly.CreateForAssembly(pluginDll);

                var result = pluginAssembly.Initialise();

                if (result.Plugins.Count > 0)
                {
                    foreach (var plugin in result.Plugins)
                    {
                        Console.WriteLine("\t\tPlugin: {0}", plugin.PluginTypeName);
                        Console.WriteLine("\t\t  Name: {0}", plugin.Metadata.Name);
                        Console.WriteLine("\t\t  Desc: {0}", plugin.Metadata.Description);
                        Console.WriteLine("\t\t  Auth: {0}", plugin.Metadata.Author);
                        Console.WriteLine("\t\t   Ver: {0}", plugin.Metadata.Version.ToString("N1"));
                        
                        if (plugin.IsUsable)
                        {
                            Console.WriteLine("\t\tPlugin is usable");
                        }
                        else
                        {
                            Console.WriteLine("\t\tPlugin is not usable: \"{0}\"", plugin.ExclusionReason);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\t\tNo plugins found in assembly");
                }

            }

            Console.ReadLine();
        }

        private void DashedLine()
        {
            Console.WriteLine(new string(Enumerable.Repeat('*', Console.BufferWidth - 5).ToArray()));
        }
    }


}