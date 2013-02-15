using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Distrib.Communication
{
    public interface ICommsLink
    {
        CommsDirection PrimaryDirection { get; }
    }

    public enum CommsDirection
    {
        Incoming,
        Outgoing,
    }

    public interface IOutgoingCommsLink
    {
        object InvokeMethod(object[] args, [CallerMemberName] string methodName = "");
    }

    public interface IOutgoingCommsLink<T> : IOutgoingCommsLink
    {

    }

    public interface IIncomingCommsLink
    {
        void StartListening(object invokerObject);
        void StopListening();
        bool IsListening { get; }
    }

    public interface IIncomingCommsLink<T> : IIncomingCommsLink
    {
        void StartListening(T invokerObject);
    }

    public interface IIncomingCommsMessageLink
    {
        ICommsMessage ProcessMessage(object obObject, ICommsMessage msg);
    }

    public sealed class DirectInvocationMessageLink : IIncomingCommsMessageLink
    {
        public ICommsMessage ProcessMessage(object obObject, ICommsMessage msg)
        {
            switch (msg.Type)
            {
                case CommsMessageType.MethodInvoke:
                    return _handleMethodInvoke(obObject, (IMethodInvokeCommsMessage)msg);
                    break;
                case CommsMessageType.MethodInvokeResult:
                    throw new InvalidOperationException();
                case CommsMessageType.PropertyGet:
                    break;
                case CommsMessageType.PropertyGetResult:
                    throw new InvalidOperationException();
                case CommsMessageType.PropertySet:
                    break;
                case CommsMessageType.PropertySetResult:
                    throw new InvalidOperationException();
                case CommsMessageType.Exception:
                    
                    throw new InvalidOperationException();
                default:
                    throw new InvalidOperationException();
            }

            return null;
        }

        private ICommsMessage _handleMethodInvoke(object obObject, IMethodInvokeCommsMessage msg)
        {
            var result = obObject.GetType()
                .GetMethod(msg.MethodName)
                .Invoke(obObject, msg.InvokeArgs);

            var resMsg = new MethodInvokeResultCommsMessage(msg, result);

            return resMsg;
        }
    }

    public class TcpIncomingCommsLink : IIncomingCommsLink
    {
        private readonly ICommsMessageReaderWriter _messageReaderWriter;
        private readonly IIncomingCommsMessageLink _messageLink;
        private readonly int _port;

        private object _lock = new object();

        private TcpListener _tcpListener = null;

        private object _invokerObject;

        public TcpIncomingCommsLink(ICommsMessageReaderWriter messageReaderWriter, IIncomingCommsMessageLink messageLink, int port)
        {
            _messageReaderWriter = messageReaderWriter;
            _port = port;
            _messageLink = messageLink;
        }

        public void StartListening(object invokerObject)
        {
            lock (_lock)
            {
                if (_tcpListener != null)
                {
                    throw new InvalidOperationException();
                }

                _invokerObject = invokerObject;
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                _tcpListener.Start();
                _tcpListener.AcceptTcpClientAsync()
                    .ContinueWith(OnClientConnected);
            }
        }

        private void OnClientConnected(Task<TcpClient> task)
        {
            var client = task.Result;
            var stream = client.GetStream();

            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            sw.AutoFlush = true;

            var msg = _messageReaderWriter.Read(sr.ReadLine());

            sw.WriteLine(_messageReaderWriter.Write(_messageLink.ProcessMessage(_invokerObject, msg)));
        }

        public void StopListening()
        {
        }

        public bool IsListening
        {
            get { throw new NotImplementedException(); }
        }
    }

    public sealed class TcpIncomingCommsLink<T> : TcpIncomingCommsLink, IIncomingCommsLink<T>
    {
        public TcpIncomingCommsLink(ICommsMessageReaderWriter messageReaderWriter, IIncomingCommsMessageLink messageLink, int port)
            : base(messageReaderWriter, messageLink, port)
        {

        }

        public void StartListening(T invokerObject)
        {
            base.StartListening(invokerObject);
        }
    }


    public sealed class TcpOutgoingCommsLink<T> : IOutgoingCommsLink<T>
    {
        private IPAddress _address;
        private int _port;
        private ICommsMessageReaderWriter _messageReaderWriter;

        public TcpOutgoingCommsLink(IPAddress address, int port, ICommsMessageReaderWriter serDeser)
        {
            _address = address;
            _port = port;
            _messageReaderWriter = serDeser;
        }

        public object InvokeMethod(object[] args, string methodName = "")
        {
            var client = new TcpClient();
            client.Connect(_address, _port);
            var stream = client.GetStream();
            var sw = new StreamWriter(stream);
            sw.AutoFlush = true;
            var sr = new StreamReader(stream);


            sw.WriteLine(_messageReaderWriter.Write(new MethodInvokeCommsMessage(methodName, args)));
            var rep = _messageReaderWriter.Read(sr.ReadLine());

            switch (rep.Type)
            {
                case CommsMessageType.MethodInvokeResult:

                    var resMsg = (IMethodInvokeResultCommsMessage)rep;
                    return resMsg.ReturnValue;

                case CommsMessageType.Exception:

                    var exMsg = (IExceptionCommsMessage)rep;
                    throw new ApplicationException("Exception occurred on remote object: '" +
                        exMsg.ExceptionMessage + "'");

                default:

                    throw new InvalidOperationException();
            }
        }
    }



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
}
