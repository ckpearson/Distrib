using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents the method invocation result comms message
    /// </summary>
    public interface IMethodInvokeResultCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The method invoke comms message
        /// </summary>
        IMethodInvokeCommsMessage InvokeMessage { get; }

        /// <summary>
        /// The value returned by the invoked method (if any)
        /// </summary>
        object ReturnValue { get; }
    }
}
