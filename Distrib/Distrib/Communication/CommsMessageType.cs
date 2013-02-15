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
