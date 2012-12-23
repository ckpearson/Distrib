using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessJob
    {
        string Identifier { get; }
        DateTime CreationTimeStamp { get; }

        ProcessJobStatus Status { get; }
    }
}
