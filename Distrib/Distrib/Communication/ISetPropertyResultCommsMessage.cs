using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents the result comms message of setting a property
    /// </summary>
    public interface ISetPropertyResultCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The set property request comms message
        /// </summary>
        ISetPropertyCommsMessage SetMessage { get; }
    }
}
