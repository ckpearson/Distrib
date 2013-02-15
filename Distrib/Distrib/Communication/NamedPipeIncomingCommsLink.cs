using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib.Communication
{
    public class NamedPipeIncomingCommsLink : IIncomingCommsLink
    {
        private readonly ICommsMessageReaderWriter _readerWriter;
        private readonly IIncomingCommsMessageProcessor _messageProcessor;

        private readonly object _lock = new object();
        private bool _listening = false;

        private object _invokeTarget = null;

        private NamedPipeServerStream _server = null;

        private readonly string _pipeName;

        public NamedPipeIncomingCommsLink(string pipeName,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
        {
            _pipeName = pipeName;
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
                    _server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
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
    }

    public sealed class NamedPipeIncomingCommsLink<T> : NamedPipeIncomingCommsLink, IIncomingCommsLink<T>
    {
        public NamedPipeIncomingCommsLink(string pipeName,
            ICommsMessageReaderWriter readerWriter, IIncomingCommsMessageProcessor messageProcessor)
            : base(pipeName, readerWriter, messageProcessor)
        {

        }

        public void StartListening(T target)
        {
            base.StartListening(target);
        }
    }

}
