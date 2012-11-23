using Distrib.Plugins.Discovery;
using Distrib.Processes;
using Distrib.Processes.Discovery;
using Distrib.Processes.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    //[DistribPlugin(typeof(IDistribProcess),
    //"Test Process",
    //"Simple Test Process",
    //1.0,
    //"Clint Pearson")]

    [DistribProcessDetails(
        "Test Process",
        "Simple Test Process",
        1.0,
        "Clint Pearson")]

    [DistribProcessPlugin("Test process", "simple test process", 1.0, "Clint Pearson")]
    class TestProcess : MarshalByRefObject, IDistribProcess
    {
        public string SayHello()
        {
            return "Hello there!";
        }
    }
}
