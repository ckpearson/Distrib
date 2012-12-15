
using Distrib.Processes;
using Ninject;
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
            p.RunNew();
        }

        public void RunNew()
        {
            //var kernel = new StandardKernel(new[]
            //    {
                    
            //    });

            // Hacky way of just loading every single Ninject Module present in Distrib assembly
            var kernel = new StandardKernel(
                    typeof(Distrib.IOC.PluginsNinjectModule).Assembly.GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Equals(typeof(Ninject.Modules.NinjectModule)))
                    .Select(t => Activator.CreateInstance(t) as Ninject.Modules.INinjectModule)
                    .ToArray());

            foreach (var pluginDll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var asm = kernel.Get<Distrib.Plugins.IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(pluginDll);

                var res = asm.Initialise();

                if (res.HasUsablePlugins)
                {
                    var firstProc = res.UsablePlugins.DefaultIfEmpty(null)
                        .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IDistribProcess)));

                    if (firstProc == null)
                    {
                        throw new InvalidOperationException("No usable process plugin exists");
                    }

                    var inst = asm.CreatePluginInstance(firstProc);
                    inst.Initialise();

                    var proc = inst.GetUnderlyingInstance<IDistribProcess>();

                    inst.Unitialise();

                    asm.Unitialise();
                }
            }
        }

        private void DashedLine()
        {
            Console.WriteLine(new string(Enumerable.Repeat('*', Console.BufferWidth - 5).ToArray()));
        }
    }


}