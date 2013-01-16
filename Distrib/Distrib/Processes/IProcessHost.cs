using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public interface IProcessHost
    {
        void Initialise();
        void Unitialise();

        bool IsInitialised { get; }

        IEnumerable<ProcessJobField> ProcessJob(IEnumerable<ProcessJobField> inputValues = null);

        IReadOnlyList<ProcessJobField> GetInputFields();
    }
}
