using Distrib.Communication;
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
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Models
{
    public abstract class ConnectionDetails
    {
        private readonly ConnectionType _type;
        private readonly IEnumerable<ConnectionDetailComponent> _components;

        protected ConnectionDetails(ConnectionType type, IEnumerable<ConnectionDetailComponent> components)
        {
            _type = type;
            _components = components;
        }

        public ConnectionType Type
        {
            get { return _type; }
        }

        public IEnumerable<ConnectionDetailComponent> Components
        {
            get { return _components; }
        }

        public abstract IIncomingCommsLink<TComms> CreateIncomingLink<TComms>() where TComms : class;
    }

    public sealed class ConnectionDetailComponent : IDataErrorInfo, INotifyPropertyChanged
    {
        private readonly string _name;
        private object _value;
        private readonly Func<object, string> _validate;
        private readonly bool _isEdit;

        public ConnectionDetailComponent(string name, object value, bool isEdit, Func<object, string> validate)
        {
            _name = name;
            _value = value;
            _validate = validate;
            _isEdit = isEdit;
        }

        public string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        public bool IsEdit
        {
            get { return _isEdit; }
        }

        private string _error;

        public string Error
        {
            get { return _error; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Value")
                {
                    if (_validate != null)
                    {
                        _error = _validate(Value);
                        return _error;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public sealed class TcpConnectionDetails : ConnectionDetails
    {
        Distrib.Communication.TCPEndpointDetails TcpEndpoint { get; set; }

        private const string IPCompName = "IP";
        private const string PortCompName = "Port";

        public TcpConnectionDetails()
            : base(ConnectionType.TCP, new[]
            {
                new ConnectionDetailComponent(IPCompName, IPAddress.Loopback, false, null),
                new ConnectionDetailComponent(PortCompName, 8080, true, OnValidatePort),
            })
        {

        }

        private static string OnValidatePort(object arg)
        {
            int port = 0;

            if (arg is string)
            {
                if (int.TryParse(arg as string, out port) == false)
                {
                    return string.Format("Could not convert '{0}' to int", arg as string);
                }
            }
            else
            {
                if (!(arg is int))
                {
                    return string.Format("Argument wasn't a string or an int");
                }

                port = (int)arg;
            }

            // Now have the port

            if (port <= 0)
            {
                return "Port must be a positive integer";
            }

            // Now check if the port is available

            if (IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .Any(c => c.LocalEndPoint.Port == port))
            {
                return "That port is currently in use";
            }

            return null;
        }

        public override IIncomingCommsLink<TComms> CreateIncomingLink<TComms>()
        {
            return new TcpIncomingCommsLink<TComms>(new TCPEndpointDetails()
            {
                Address = IPAddress.Loopback,
                Port = (int)base.Components.Single(c => c.Name == PortCompName).Value,
            },
            new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
            new DirectInvocationCommsMessageProcessor());
        }
    }

    public sealed class NamedPipeConnectionDetails : ConnectionDetails
    {
        Distrib.Communication.NamedPipeEndpointDetails NamedPipeEndpoint { get; set; }

        private const string MachineCompName = "Machine";
        private const string PipeCompName = "Pipe Name";

        public NamedPipeConnectionDetails()
            : base(ConnectionType.NamedPipe,
            new[]
            {
                new ConnectionDetailComponent(MachineCompName, Dns.GetHostName(), false, null),
                new ConnectionDetailComponent(PipeCompName, "NodePipe", true, OnValidatePipeName),
            })
        {

        }

        private static string OnValidatePipeName(object arg)
        {
            var s = (string)arg;
            if (string.IsNullOrEmpty(s))
            {
                return "Pipe name must be supplied";
            }

            return null;
        }

        public override IIncomingCommsLink<TComms> CreateIncomingLink<TComms>()
        {
            return new NamedPipeIncomingCommsLink<TComms>(new NamedPipeEndpointDetails()
            {
                MachineName = null,
                PipeName = (string)base.Components.Single(c => c.Name == PipeCompName).Value,
            }, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()),
            new DirectInvocationCommsMessageProcessor());
        }
    }

    public enum ConnectionType
    {
        TCP,
        NamedPipe,
    }
}
