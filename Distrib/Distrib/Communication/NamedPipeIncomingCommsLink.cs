﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    [Serializable()]
    public sealed class NamedPipeEndpointDetails
    {
        public string MachineName { get; set; }
        public string PipeName { get; set; }
    }

    public class NamedPipeIncomingCommsLink : IIncomingCommsLink
    {
        protected readonly ICommsMessageReaderWriter _readerWriter;
        protected readonly IIncomingCommsMessageProcessor _messageProcessor;

        private readonly object _lock = new object();
        private bool _listening = false;

        private object _invokeTarget = null;

        private NamedPipeServerStream _server = null;

        private readonly NamedPipeEndpointDetails _endpoint;

        public NamedPipeIncomingCommsLink(NamedPipeEndpointDetails endpoint,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
        {
            _endpoint = endpoint;
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
                    _server = new NamedPipeServerStream(_endpoint.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    Task.Factory.StartNew(() =>
                        {
                            _server.WaitForConnection();
                            OnClientConnected();
                        });

                    _listening = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to start listening", ex);
            }
        }

        private void OnClientConnected()
        {
            try
            {
                lock (_lock)
                {
                    if (!_server.IsConnected)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            _server.WaitForConnection();
                            OnClientConnected();
                        });
                    }

                    var sr = new StreamReader(_server);
                    var sw = new StreamWriter(_server);
                    sw.AutoFlush = true;

                    var incoming = _readerWriter.Read(sr.ReadLine());
                    sw.WriteLine(_readerWriter.Write(_messageProcessor.ProcessMessage(_invokeTarget, incoming)));

                    _server.WaitForPipeDrain();
                    _server.Disconnect();

                    Task.Factory.StartNew(() =>
                    {
                        _server.WaitForConnection();
                        OnClientConnected();
                    });

                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to handle connected client", ex);
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

                    if (_server.IsConnected)
                    {
                        _server.Disconnect();
                    }

                    _server.Close();
                    _server = null;
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
            var endp = (NamedPipeEndpointDetails)endpoint;
            return new NamedPipeOutgoingCommsLink(endp.MachineName, endp.PipeName,
                _readerWriter);
        }
    }

    public sealed class NamedPipeIncomingCommsLink<T> : NamedPipeIncomingCommsLink, IIncomingCommsLink<T> where T : class
    {
        public NamedPipeIncomingCommsLink(NamedPipeEndpointDetails endpoint,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
            : base(endpoint, readerWriter, messageProcessor)
        {

        }

        public void StartListening(T target)
        {
            base.StartListening(target);
        }


        public new IOutgoingCommsLink<T> CreateOutgoingOfSameTransport(object endpoint)
        {
            var e = (NamedPipeEndpointDetails)endpoint;
            return new NamedPipeOutgoingCommsLink<T>(e.MachineName, e.PipeName, _readerWriter);
        }


        public IOutgoingCommsLink<K> CreateOutgoingOfSameTransportDiffContract<K>(object endpoint) where K : class
        {
            var e = (NamedPipeEndpointDetails)endpoint;
            return new NamedPipeOutgoingCommsLink<K>(e.MachineName, e.PipeName, _readerWriter);
        }
    }

}
