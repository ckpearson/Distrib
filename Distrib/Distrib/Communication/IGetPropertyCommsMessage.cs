using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a comms message for retrieving a property value
    /// </summary>
    public interface IGetPropertyCommsMessage : ICommsMessage
    {
        /// <summary>
        /// The name of the property to get
        /// </summary>
        string PropertyName { get; }
    }
}
