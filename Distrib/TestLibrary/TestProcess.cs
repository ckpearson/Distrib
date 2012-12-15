using Distrib.Plugins;
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

    //[DistribProcessDetails(
    //    "Test Process",
    //    "Simple Test Process",
    //    1.0,
    //    "Clint Pearson")]

    //[DistribProcessPlugin("Test process", "simple test process", 1.0, "Clint Pearson", "abc")]
    //class TestProcess : MarshalByRefObject, IDistribPlugin, IDistribProcess
    //{
    //    void IDistribPlugin.InitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
    //    {
    //        // Do some plugin initialisation here

    //    }

    //    void IDistribPlugin.UninitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
    //    {
    //        // Do some plugin unitialisation here
    //    }

    //    string IDistribProcess.SayHello()
    //    {
    //        return "HELLO";
    //    }
    //}

    //[DistribProcessPlugin("second process", "second process", 3.0, "Clint Pearson", "abc")]
    //class SecondProcess : MarshalByRefObject, IDistribPlugin, IDistribProcess
    //{
    //    public void InitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void UninitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public string SayHello()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [DistribProcessPlugin("New test process", "new process for the new plugin system", 1.0, "Clint Pearson", "identifier")]
    public class NewTestProcess : MarshalByRefObject, IPlugin, IDistribProcess
    {
        public string SayHello()
        {
            throw new NotImplementedException();
        }

        public void InitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }

        public void UninitialisePlugin(IPluginInteractionLink interactionLink)
        {
        }
    }
}
