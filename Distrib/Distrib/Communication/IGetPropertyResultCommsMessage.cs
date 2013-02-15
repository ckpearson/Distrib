using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents the result of a property retrieval comms message
    /// </summary>
    public interface IGetPropertyResultCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The original message requesting the property get
        /// </summary>
        IGetPropertyCommsMessage PropertyGetMessage { get; }

        /// <summary>
        /// The value of the property
        /// </summary>
        object Value { get; }
    }
}
