using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public enum ProcessJobStatus
    {
        Unknown = 0,

        /// <summary>
        /// Waiting to be processed (the process has it in its queue)
        /// </summary>
        AwaitingProcessing,

        /// <summary>
        /// Currently being processed
        /// </summary>
        BeingProcessed,

        /// <summary>
        /// Awaiting the finalisation of the job by the current process
        /// </summary>
        AwaitingFinalisation,

        /// <summary>
        /// Awaiting the handoff to the next process in the chain
        /// </summary>
        AwaitingHandoff,

        /// <summary>
        /// Awaiting the complete finalisation of the job handling
        /// and the result being collected culminating in the final
        /// destruction of the job.
        /// </summary>
        AwaitingEnd,
    }
}
