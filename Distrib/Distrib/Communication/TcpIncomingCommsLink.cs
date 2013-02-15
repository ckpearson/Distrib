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
    /// <summary>
    /// Incoming comms link that uses TCP
    /// </summary>
    public class TcpIncomingCommsLink : IIncomingCommsLink
    {
        private readonly ICommsMessageReaderWriter _readerWriter;
        private readonly IIncomingCommsMessageProcessor _messageProcessor;

        private readonly int _listeningPort;
        private readonly IPAddress _listeningAddress;

        private readonly object _lock = new object();
        private bool _listening = false;

        private object _invokeTarget = null;

        private TcpListener _listener = null;

        public TcpIncomingCommsLink(IPAddress listeningAddress, int listeningPort,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
        {
            _listeningAddress = listeningAddress;
            _listeningPort = listeningPort;
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
                    _listener = new TcpListener(_listeningAddress, _listeningPort);
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
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to handle connected client", ex);
            }
        }
    }

    /// <summary>
    /// Incoming comms link that uses TCP with a type parameter for compile-time checks
    /// </summary>
    /// <typeparam name="T">The comms interface</typeparam>
    public sealed class TcpIncomingCommsLink<T> : TcpIncomingCommsLink, IIncomingCommsLink<T>
    {
        public TcpIncomingCommsLink(IPAddress listeningAddress, int listeningPort,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
            : base(listeningAddress, listeningPort, readerWriter, messageProcessor)
        {

        }

        public void StartListening(T invokerObject)
        {
            base.StartListening(invokerObject);
        }
    }
}
