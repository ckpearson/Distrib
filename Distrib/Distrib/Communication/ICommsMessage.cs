﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents a communications message
    /// </summary>
    public interface ICommsMessage
    {
        /// <summary>
        /// Gets the type of comms message
        /// </summary>
        CommsMessageType Type { get; }
    }
}