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
        private readonly string _name;

        public DirectInvokeCommsBridge(string name)
        {
            _name = name;
        }

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

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public event Func<ICommsMessage, ICommsMessage> MessageReceived;
    }
}
