﻿/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    public abstract class OutgoingCommsProxyBase
    {
        private IOutgoingCommsLink _outgoingLink;

        public OutgoingCommsProxyBase(IOutgoingCommsLink outgoingLink)
        {
            _outgoingLink = outgoingLink;
        }

        public IOutgoingCommsLink Link
        {
            get { return _outgoingLink; }
        }
    }

    /// <summary>
    /// Helper base for outgoing comms proxies
    /// </summary>
    public abstract class OutgoingCommsProxyBase<T> : OutgoingCommsProxyBase where T : class
    {
        public OutgoingCommsProxyBase(IOutgoingCommsLink<T> outgoingLink)
            : base(outgoingLink)
        {
        }

        /// <summary>
        /// The outgoing comms link
        /// </summary>
        public new IOutgoingCommsLink<T> Link
        {
            get { return (IOutgoingCommsLink<T>)base.Link; }
        }
    }
}
