using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents the primary direction that a comms link operates in
    /// </summary>
    public enum CommsDirection
    {
        /// <summary>
        /// The comms link actively listens to incoming requests
        /// </summary>
        Incoming,

        /// <summary>
        /// The comms link actively generates and communicates requests
        /// </summary>
        Outgoing,
    }
}
