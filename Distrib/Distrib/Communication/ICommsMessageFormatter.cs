using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Responsible for serialising and deserialising a comms message to and from a byte array
    /// </summary>
    public interface ICommsMessageFormatter
    {
        /// <summary>
        /// Serialises the comms message
        /// </summary>
        /// <param name="message">The comms message</param>
        /// <returns>The serialised byte array</returns>
        byte[] Serialise(ICommsMessage message);

        /// <summary>
        /// Deserialises a comms message
        /// </summary>
        /// <param name="data">The byte array to deserialise</param>
        /// <returns>The deserialised comms message</returns>
        ICommsMessage Deserialise(byte[] data);
    }
}
