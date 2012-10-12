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
            var dir = @"C:\Users\Clint\Desktop\distrib plugins\";

            foreach (var plugindll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var dp = DistribPluginFactory.PluginFromAssembly(plugindll);
                dp.Initialise();
                dp.Uninitialise();
                File.Delete(plugindll);
            }
        }
    }
}