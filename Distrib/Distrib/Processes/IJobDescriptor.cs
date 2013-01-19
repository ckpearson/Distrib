using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Holds the core details of the job
    /// </summary>
    public interface IJobDescriptor
    {
        /// <summary>
        /// Gets the job name
        /// </summary>
        string JobName { get; }

        /// <summary>
        /// Gets the input definition fields
        /// </summary>
        IReadOnlyList<IProcessJobDefinitionField> InputFields { get; }

        /// <summary>
        /// Gets the output definition fields
        /// </summary>
        IReadOnlyList<IProcessJobDefinitionField> OutputFields { get; }
    }
}
