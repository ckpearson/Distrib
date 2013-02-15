using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Distrib.Communication
{
    /// <summary>
    /// Responsible for writing and reading comms messages to and from strings respectively
    /// </summary>
    public interface ICommsMessageReaderWriter
    {
        /// <summary>
        /// Writes the comms message to a string
        /// </summary>
        /// <param name="message">The comms message</param>
        /// <returns>The string to be transmitted</returns>
        string Write(ICommsMessage message);

        /// <summary>
        /// Reads a comms message from a string
        /// </summary>
        /// <param name="data">The string to read the comms message from</param>
        /// <returns>The resultant comms message</returns>
        ICommsMessage Read(string data);
    }
}
