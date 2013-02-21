/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
