using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class TCPEndpointDetails
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
    }

    /// <summary>
    /// Incoming comms link that uses TCP
    /// </summary>
    public class TcpIncomingCommsLink : IIncomingCommsLink
    {
        protected readonly ICommsMessageReaderWriter _readerWriter;
        protected readonly IIncomingCommsMessageProcessor _messageProcessor;

        private readonly TCPEndpointDetails _endpoint;

        private readonly object _lock = new object();
        private bool _listening = false;

        private object _invokeTarget = null;

        private TcpListener _listener = null;

        public TcpIncomingCommsLink(TCPEndpointDetails endpointDetails,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
        {
            _endpoint = endpointDetails;
            _readerWriter = readerWriter;
            _messageProcessor = messageProcessor;
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

                    _invokeTarget = target;
                    _listener = new TcpListener(_endpoint.Address, _endpoint.Port);
                    _listener.Start();
                    _listening = true;
                    _listener.AcceptTcpClientAsync()
                        .ContinueWith(OnClientConnected);
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

                    _listener.Stop();
                    _listener = null;
                    _invokeTarget = null;
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

        private void OnClientConnected(Task<TcpClient> task)
        {
            try
            {
                lock (_lock)
                {
                    var client = task.Result;
                    var stream = client.GetStream();
                    var sr = new StreamReader(stream);
                    var sw = new StreamWriter(stream);
                    sw.AutoFlush = true;

                    var incomingMsg = _readerWriter.Read(sr.ReadLine());
                    sw.WriteLine(_readerWriter.Write(_messageProcessor.ProcessMessage(_invokeTarget, incomingMsg)));

                    client.Close();
                    _listener.AcceptTcpClientAsync()
                        .ContinueWith(OnClientConnected);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to handle connected client", ex);
            }
        }

        public CommsDirection PrimaryDirection
        {
            get { return CommsDirection.Incoming; }
        }

        public object GetEndpointDetails()
        {
            return _endpoint;
        }


        public IOutgoingCommsLink CreateOutgoingOfSameTransport(object endpoint)
        {
            var endp = (TCPEndpointDetails)endpoint;
            return new TcpOutgoingCommsLink(endp.Address, endp.Port,
                _readerWriter);
        }
    }

    /// <summary>
    /// Incoming comms link that uses TCP with a type parameter for compile-time checks
    /// </summary>
    /// <typeparam name="T">The comms interface</typeparam>
    public sealed class TcpIncomingCommsLink<T> : TcpIncomingCommsLink, IIncomingCommsLink<T> where T : class
    {
        public TcpIncomingCommsLink(TCPEndpointDetails endpoint,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
            : base(endpoint, readerWriter, messageProcessor)
        {

        }

        public void StartListening(T invokerObject)
        {
            base.StartListening(invokerObject);
        }


        public new IOutgoingCommsLink<T> CreateOutgoingOfSameTransport(object endpoint)
        {
            var e = (TCPEndpointDetails)endpoint;
            return new TcpOutgoingCommsLink<T>(e.Address, e.Port, _readerWriter);
        }

        public IOutgoingCommsLink<K> CreateOutgoingOfSameTransportDiffContract<K>(object endpoint) where K : class
        {
            var e = (TCPEndpointDetails)endpoint;
            return new TcpOutgoingCommsLink<K>(e.Address, e.Port, _readerWriter);
        }
    }
}
