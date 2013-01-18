using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Represents a job for the process system
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets the input tracker
        /// </summary>
        IJobInputTracker InputTracker { get; }

        /// <summary>
        /// Gets the output tracker
        /// </summary>
        IJobOutputTracker OutputTracker { get; }
    }
}
