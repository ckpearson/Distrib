using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a comms message requesting a property be set
    /// </summary>
    public interface ISetPropertyCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The name of the property to set
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// The value to set the property to
        /// </summary>
        object Value { get; }
    }
}
