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
                    //sw.WriteLine(_readerWriter.Write(_messageProcessor.ProcessMessage(_invokeTarget, incoming)));
                    try
                    {
                        sw.WriteLine(_readerWriter.Write(_messageProcessor.ProcessMessage(_invokeTarget, incoming)));
                    }
                    catch (Exception ex)
                    {
                        if (_server != null && _server.IsConnected)
                        {
                            try
                            {
                                sw.WriteLine(_readerWriter.Write(new ExceptionCommsMessage(incoming, ex)));
                            }
                            catch { }
                        }
                        throw new ApplicationException("Failed to write processed response", ex);
                    }
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
