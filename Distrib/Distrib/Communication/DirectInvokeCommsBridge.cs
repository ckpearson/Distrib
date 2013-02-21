/*
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
