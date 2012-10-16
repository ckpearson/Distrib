using Distrib.Plugins.Discovery;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
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
