
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
            //p.RunProcessSubsystemTest();
            p.RunProcessHostTest();
        }

        private void RunProcessHostTest()
        {
            var kernel = kernelGet();

            var asmFile = Directory.EnumerateFiles(dir, "*.dll").DefaultIfEmpty(null).FirstOrDefault();
            if (asmFile == null)
                throw new InvalidOperationException("No assemblies found in directory");


            var pluginAsm = kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(asmFile);


            var initRes = pluginAsm.Initialise();

            try
            {
                if (!initRes.HasUsablePlugins) throw new ApplicationException();

                var fpp = initRes.UsablePlugins
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IProcess)));

                if (fpp == null) throw new ArgumentNullException();

                var prochost = kernel.Get<IProcessHostFactory>().CreateHostFromPluginInDomain(fpp);
                
            }
            catch (Exception)
            {   
                throw;
            }
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
                    .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IProcess)));

                if (firstProcPlugin == null)
                    throw new InvalidOperationException("No process plugin present in plugin assembly");
                

                var manPluginInst = pluginAsm.CreatePluginInstance(firstProcPlugin);

                manPluginInst.Initialise();

                IProcess procInstance =
                    manPluginInst.GetUnderlyingInstance<IProcess>();

                var jd = procInstance.JobDefinition as object;
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

        private void DashedLine()
        {
            Console.WriteLine(new string(Enumerable.Repeat('*', Console.BufferWidth - 5).ToArray()));
        }
    }


}