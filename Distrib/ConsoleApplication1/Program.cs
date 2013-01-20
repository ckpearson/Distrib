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

using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

            //var asmFile = Directory.EnumerateFiles(dir, "*.dll").DefaultIfEmpty(null).FirstOrDefault();
            //if (asmFile == null)
            //    throw new InvalidOperationException("No assemblies found in directory");

            var asmFile = Path.Combine(dir, "TestLibrary.dll");


            var pluginAsm = kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(asmFile);


            var initRes = pluginAsm.Initialise();

            try
            {
                if (!initRes.HasUsablePlugins) throw new ApplicationException();

                var fpp = initRes.UsablePlugins
                    .DefaultIfEmpty(null)
                    .FirstOrDefault(p => p.Metadata.InterfaceType.Equals(typeof(IProcess)));

                if (fpp == null) throw new ArgumentNullException();

                var procHost = kernel.Get<IProcessHostFactory>()
                    .CreateHostFromPlugin(fpp);

                procHost.Initialise();

                var inputFields = procHost.JobDescriptor.OutputFields;

                var sw = new Stopwatch();
                sw.Start();
                var outputs = procHost.ProcessJob(null);
                sw.Stop();

                Console.WriteLine("Process took '{0}' to execute", sw.Elapsed);


                procHost.Unitialise();
            }
            catch (Exception)
            {
                throw;
            }

            Console.ReadLine();
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
                }
                catch { }
            }
        }

        private void DashedLine()
        {
            Console.WriteLine(new string(Enumerable.Repeat('*', Console.BufferWidth - 5).ToArray()));
        }
    }


}