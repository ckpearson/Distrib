using Distrib.Plugins;
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
        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public void Run()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "distrib plugins");

            foreach (var pluginDll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var pluginAssembly = DistribPluginAssembly.CreateForAssembly(pluginDll);

                var result = pluginAssembly.Initialise();

               

            }
        }
    }


}