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
    /// Represents an incoming comms link for direct invocation used when objects are reachable directly
    /// </summary>
    public class DirectInvokeIncomingCommsLink : IIncomingCommsLink
    {
        protected readonly DirectInvocationCommsMessageProcessor _processor;
        protected readonly DirectInvokeCommsBridge _bridge;

        private object _target;

        private readonly object _lock = new object();

        private bool _listening = false;

        public DirectInvokeIncomingCommsLink(DirectInvokeCommsBridge bridge)
        {
            bridge.MessageReceived += bridge_MessageReceived;
            _processor = new DirectInvocationCommsMessageProcessor();
            _bridge = bridge;
        }

        ICommsMessage bridge_MessageReceived(ICommsMessage msg)
        {
            lock (_lock)
            {
                if (!IsListening)
                {
                    throw new InvalidOperationException("Direct incoming link received a message but isn't listening");
                }

                return _processor.ProcessMessage(_target, msg); 
            }
        }

        public void StartListening(object target)
        {
            try
            {
                lock (_lock)
                {
                    if (IsListening)
                    {
                        throw new InvalidOperationException("Already listening");
                    }

                    _target = target;
                    _listening = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to start listening", ex);
            }
        }

        public void StopListening()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsListening)
                    {
                        throw new InvalidOperationException("Not currently listening");
                    }

                    _target = null;
                    _listening = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to stop listening", ex);
            }
        }

        public bool IsListening
        {
            get
            {
                lock (_lock)
                {
                    return _listening;
                }
            }
        }

        public CommsDirection PrimaryDirection
        {
            get { return CommsDirection.Incoming; }
        }


        public object GetEndpointDetails()
        {
            return _bridge;
        }


        public IOutgoingCommsLink CreateOutgoingOfSameTransport(object endpoint)
        {
            return new DirectInvokeOutgoingCommsLink(_bridge);
        }
    }

    public class DirectInvokeIncomingCommsLink<T> : DirectInvokeIncomingCommsLink, IIncomingCommsLink<T> where T : class
    {
        public DirectInvokeIncomingCommsLink(DirectInvokeCommsBridge bridge)
            : base(bridge) { }

        public void StartListening(T target)
        {
            base.StartListening(target);
        }


        public new IOutgoingCommsLink<T> CreateOutgoingOfSameTransport(object endpoint)
        {
            return new DirectInvokeOutgoingCommsLink<T>((DirectInvokeCommsBridge)endpoint);
        }


        public IOutgoingCommsLink<K> CreateOutgoingOfSameTransportDiffContract<K>(object endpoint) where K : class
        {
            return new DirectInvokeOutgoingCommsLink<K>((DirectInvokeCommsBridge)endpoint);
        }
    }
}
