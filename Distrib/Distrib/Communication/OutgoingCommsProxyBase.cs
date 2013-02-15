using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    /// <summary>
    /// Helper base for outgoing comms proxies
    /// </summary>
    public abstract class OutgoingCommsProxyBase
    {
        private IOutgoingCommsLink _outgoingLink;

        public OutgoingCommsProxyBase(IOutgoingCommsLink outgoingLink)
        {
            _outgoingLink = outgoingLink;
        }

        /// <summary>
        /// The outgoing comms link
        /// </summary>
        public IOutgoingCommsLink Link
        {
            get { return _outgoingLink; }
        }
    }
}
