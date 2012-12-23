﻿
using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Ninject;
using Ninject.Modules;
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
        private Func<StandardKernel> kernelGet = new Func<StandardKernel>(() =>
            {
                return new StandardKernel(
                    typeof(PluginsNinjectModule).Assembly.GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Equals(typeof(NinjectModule)))
                    .Select(t => Activator.CreateInstance(t) as INinjectModule)
                    .ToArray());
            });

        static void Main(string[] args)
        {
            var p = new Program();
            p.RunProcessTest();
        }

        private void RunProcessTest()
        {
            var kernel = kernelGet();

            var asmFile = Directory.EnumerateFiles(dir, "*.dll").DefaultIfEmpty(null).FirstOrDefault();
            if (asmFile == null)
                throw new InvalidOperationException("No assemblies found in directory");

            var pluginAsm = kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(asmFile);

            var initRes = pluginAsm.Initialise();

            try
            {
                if (!initRes.HasUsablePlugins)
                    throw new InvalidOperationException("Plugin assembly contains no usable plugins");

                var firstProcPlugin = initRes.UsablePlugins
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IDistribProcess)));

                if (firstProcPlugin == null)
                    throw new InvalidOperationException("No process plugin present in plugin assembly");

                var manPluginInst = pluginAsm.CreatePluginInstance(firstProcPlugin);

                manPluginInst.Initialise();

                IDistribProcess procInstance =
                    manPluginInst.GetUnderlyingInstance<IDistribProcess>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error", ex);
            }
            finally
            {
                try
                {
                    if (pluginAsm != null && pluginAsm.IsInitialised)
                        pluginAsm.Unitialise();
                }catch { }
            }
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

                    var msg = proc.SayHello();

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