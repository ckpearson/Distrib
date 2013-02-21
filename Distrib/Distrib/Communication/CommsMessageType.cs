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
    /// The type of the comms message
    /// </summary>
    public enum CommsMessageType
    {
        /// <summary>
        /// A request to invoke a method is being made
        /// </summary>
        MethodInvoke,

        /// <summary>
        /// The method has been invoked and the result is being returned
        /// </summary>
        MethodInvokeResult,

        /// <summary>
        /// A request to get the value of a property is being made
        /// </summary>
        PropertyGet,

        /// <summary>
        /// The property value has been retrieved and the result is being returned
        /// </summary>
        PropertyGetResult,

        /// <summary>
        /// A request to set the value of a property is being made
        /// </summary>
        PropertySet,

        /// <summary>
        /// The property value has been set and the result is being returned
        /// </summary>
        PropertySetResult,

        /// <summary>
        /// A comms request has led to an exception and this is being returned to the caller
        /// </summary>
        Exception,

        /// <summary>
        /// The type of the request is unknown
        /// </summary>
        Unknown,
    }
}
