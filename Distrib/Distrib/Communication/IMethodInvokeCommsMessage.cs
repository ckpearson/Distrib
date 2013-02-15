using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a comms message for invoking a method
    /// </summary>
    public interface IMethodInvokeCommsMessage : ICommsMessage
    {
        /// <summary>
        /// Gets the name of the method to invoke
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Gets the arguments to invoke the method with
        /// </summary>
        object[] InvokeArgs { get; }
    }
}
