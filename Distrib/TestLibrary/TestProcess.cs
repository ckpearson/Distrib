﻿using Distrib.Plugins_old;
using Distrib.Plugins_old.Discovery;
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

    [DistribProcessPlugin("Test process", "simple test process", 1.0, "Clint Pearson", "{C068F971-7722-4CE1-81F5-E0A548F383DD}")]
    class TestProcess : MarshalByRefObject, IDistribPlugin, IDistribProcess
    {
        void IDistribPlugin.InitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
        {
            // Do some plugin initialisation here

        }

        void IDistribPlugin.UninitPlugin(Distrib.Plugins_old.Controllers.IDistribPluginControllerInterface cont)
        {
            // Do some plugin unitialisation here
        }

        string IDistribProcess.SayHello()
        {
            return "HELLO";
        }
    }
}