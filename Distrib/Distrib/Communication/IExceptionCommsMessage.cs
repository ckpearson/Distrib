using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a comms message that is sent when an exception occurs during message processing
    /// </summary>
    public interface IExceptionCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The comms message that was being processed
        /// </summary>
        ICommsMessage CausingMessage { get; }

        /// <summary>
        /// The exception that was thrown
        /// </summary>
        Exception Exception { get; }
    }
}
