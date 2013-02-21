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
    /// Comms message for when an exception occurred during message processing
    /// </summary>
    [Serializable()]
    public sealed class ExceptionCommsMessage : CommsMessageBase, IExceptionCommsMessage
    {
        private readonly ICommsMessage _causingMessage;
        private readonly Exception _exception;

        public ExceptionCommsMessage(ICommsMessage causingMessage, Exception exception)
            : base(CommsMessageType.Exception)
        {
            _causingMessage = causingMessage;
            _exception = exception;
        }

        public ICommsMessage CausingMessage
        {
            get { return _causingMessage; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
