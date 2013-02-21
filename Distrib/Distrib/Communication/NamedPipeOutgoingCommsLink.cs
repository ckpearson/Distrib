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
using System.Threading.Tasks;

namespace Distrib.Communication
{
    public class NamedPipeOutgoingCommsLink : IOutgoingCommsLink
    {
        private readonly string _serverName;
        private readonly string _pipeName;

        private readonly ICommsMessageReaderWriter _readerWriter;

        public NamedPipeOutgoingCommsLink(string serverName, string pipeName, ICommsMessageReaderWriter readerWriter)
        {
            if (string.IsNullOrEmpty(serverName)) throw Ex.ArgNull(() => serverName);
            if (string.IsNullOrEmpty(pipeName)) throw Ex.ArgNull(() => pipeName);
            if (readerWriter == null) throw Ex.ArgNull(() => readerWriter);

            _serverName = serverName;
            _pipeName = pipeName;
            _readerWriter = readerWriter;
        }

        private ICommsMessage _communicate(ICommsMessage message, CommsMessageType lookingFor = CommsMessageType.Unknown)
        {
            try
            {
                var client = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut);
                client.Connect();
                var sw = new StreamWriter(client);
                var sr = new StreamReader(client);
                sw.AutoFlush = true;
                sw.WriteLine(_readerWriter.Write(message));
                client.WaitForPipeDrain();
                var reply = _readerWriter.Read(sr.ReadLine());
                client.Close();

                if (reply.Type == lookingFor || lookingFor == CommsMessageType.Unknown)
                {
                    return reply;
                }
                else
                {
                    if (reply.Type == CommsMessageType.Exception)
                    {
                        throw new ApplicationException("An exception occurred on the remote object when processing request",
                            ((IExceptionCommsMessage)reply).Exception);
                    }
                    else
                    {
                        throw new ApplicationException("An unexpected message type was received: '" +
                            reply.Type + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to communicate message over outgoing link", ex);
            }
        }

        private T _communicate<T>(ICommsMessage message, CommsMessageType lookingFor = CommsMessageType.Unknown)
        {
            return (T)_communicate(message, lookingFor);
        }

        public object InvokeMethod(object[] args, string methodName = "")
        {
            if (string.IsNullOrEmpty(methodName)) throw Ex.ArgNull(() => methodName);

            try
            {
                var result = _communicate<IMethodInvokeResultCommsMessage>
                    (new MethodInvokeCommsMessage(methodName, args), CommsMessageType.MethodInvokeResult);

                return result.ReturnValue;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to invoke method over outgoing comms link", ex);
            }
        }

        public TRes InvokeMethod<TRes>(object[] args, string methodName = "")
        {
            return (TRes)InvokeMethod(args, methodName);
        }

        public object GetProperty(string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName)) throw Ex.ArgNull(() => propertyName);

            try
            {
                var result = _communicate<IGetPropertyResultCommsMessage>
                    (new GetPropertyCommsMessage(propertyName), CommsMessageType.PropertyGetResult);

                return result.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get property over outgoing comms link", ex);
            }
        }

        public void SetProperty(object value, string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName)) throw Ex.ArgNull(() => propertyName);

            try
            {
                var result = _communicate<ISetPropertyResultCommsMessage>
                    (new SetPropertyCommsMessage(propertyName, value), CommsMessageType.PropertySetResult);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to set property over outgoing comms link", ex);
            }
        }

        public CommsDirection PrimaryDirection
        {
            get { return CommsDirection.Outgoing; }
        }
    }

    public sealed class NamedPipeOutgoingCommsLink<T> : NamedPipeOutgoingCommsLink, IOutgoingCommsLink<T> where T : class
    {
        public NamedPipeOutgoingCommsLink(string serverName, string pipeName, ICommsMessageReaderWriter readerWriter)
            : base(serverName, pipeName, readerWriter)
        {

        }
    }
}
