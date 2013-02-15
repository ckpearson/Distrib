using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Simple shared publish / subscribe object used for the direct invoke comms links
    /// </summary>
    public sealed class DirectInvokeCommsBridge
    {
        public ICommsMessage SendMessage(ICommsMessage msg)
        {
            if (this.MessageReceived != null)
            {
                return this.MessageReceived(msg);
            }
            else
            {
                throw new InvalidOperationException("No objects are subscribed to the event");
            }
        }

        public event Func<ICommsMessage, ICommsMessage> MessageReceived;
    }
}
