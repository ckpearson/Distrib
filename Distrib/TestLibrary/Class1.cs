using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public sealed class TestDistribProcess : IDistribProcess
    {

        public string SayHello()
        {
            return "Howdy!";
        }
    }
}
