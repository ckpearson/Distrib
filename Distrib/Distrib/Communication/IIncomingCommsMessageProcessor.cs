using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Represents an incoming comms message processor
    /// </summary>
    public interface IIncomingCommsMessageProcessor
    {
        ICommsMessage ProcessMessage(object target, ICommsMessage msg);
    }
}
