using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Helper base for comms message implementations
    /// </summary>
    [Serializable()]
    public abstract class CommsMessageBase : ICommsMessage
    {
        private readonly CommsMessageType _type;

        public CommsMessageBase(CommsMessageType messageType)
        {
            _type = messageType;
        }

        public CommsMessageType Type
        {
            get { return _type; }
        }
    }
}
