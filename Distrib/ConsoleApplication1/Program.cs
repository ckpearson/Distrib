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

            var b = DistribPluginTypes.IsTypeAValidPlugin<TestDistribProcess>();

            foreach (var plugindll in Directory.EnumerateFiles(dir, "*.dll"))
            {
                var dp = DistribPluginFactory.PluginFromAssembly(plugindll);
                dp.Initialise();
                dp.Uninitialise();
                File.Delete(plugindll);
            }
        }
    }

    [DistribPluginDetails(
        pluginInterfaceType: typeof(IDistribProcess),
        name: "Test Distrib Process",
        description: "A test process for Distrib",
        version: 1.0,
        author: "Clint Pearson",
        copyright: "Clint Pearson 2012")]
    public sealed class TestDistribProcess : IDistribProcess
    {

        public string SayHello()
        {
            return "Howdy!";
        }
    }
}