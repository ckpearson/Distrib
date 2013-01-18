using Distrib.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IJobFactory
    {
        IJob CreateJob(
            [IOC(false)] IJobInputTracker inputTracker, 
            [IOC(false)] IJobOutputTracker outputTracker, 
            [IOC(false)] IJobDefinition jobDefinition);
    }
}
