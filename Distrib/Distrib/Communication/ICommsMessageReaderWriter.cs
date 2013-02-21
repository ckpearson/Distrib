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
