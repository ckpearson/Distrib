using Distrib.Communication;
using Distrib.Nodes.Process;
using DistribApps.Comms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Comms.TcpProvider
{
    [Export(typeof(ICommsProvider<Distrib.Nodes.Process.IProcessNodeComms>))]
    public sealed class TcpCommsProvider : 
        ICommsProvider<Distrib.Nodes.Process.IProcessNodeComms>
    {
        public IIncomingCommsLink<Distrib.Nodes.Process.IProcessNodeComms> CreateIncomingLink(CommsEndpointDetails endpoint)
        {
            if (endpoint is ProcessNode.Comms.TcpProvider.TcpEndpointDetails == false)
            {
                throw new InvalidOperationException();
            }

            var epoint = (TcpEndpointDetails)endpoint;

            try
            {
                return new TcpIncomingCommsLink<IProcessNodeComms>(
                    new TCPEndpointDetails()
                    {
                        Address = IPAddress.Loopback,
                        Port = epoint.Port,
                    }, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()), new DirectInvocationCommsMessageProcessor());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create incoming tcp comms link", ex);
            }
        }

        public IOutgoingCommsLink<Distrib.Nodes.Process.IProcessNodeComms> CreateOutgoing(CommsEndpointDetails endpoint)
        {
            throw new NotImplementedException();
        }


        public string ProviderType
        {
            get { return "TCP"; }
        }


        public CommsEndpointDetails GetEndpointDetailsItem()
        {
            return new TcpEndpointDetails(this)
            {
                Address = IPAddress.Loopback,
                Port = 8080,
            };
        }
    }

    public sealed class TcpEndpointDetails :
        CommsEndpointDetails
    {
        public const string fld_address = "Address";
        public const string fld_port = "Port";

        public TcpEndpointDetails(ICommsProvider provider)
            : base("TCP", provider)
        {

        }

        protected override IEnumerable<CommsEndpointDetailsField> CreateFields()
        {
            return new[]
            {
                // Can't edit the IP Address field because the process node app always listens on the local address
                new CommsEndpointDetailsField(fld_address, IPAddress.Loopback, false),
                new CommsEndpointDetailsField(fld_port, 8080, true)
                {
                    ValidationFunc = OnValidatePort
                },
            };
        }

        private string OnValidatePort(object port)
        {
            if (port == null)
            {
                return "A port must be specified";
            }

            int portNum = 0;
            try
            {
                portNum = Convert.ToInt32(port);
            }
            catch(Exception ex)
            {
                return "Failed to convert to number: '" + ex.Message + "'";
            }

            if (portNum <= 0)
            {
                return "Port must be a positive integer";
            }

            return null;
        }

        public IPAddress Address
        {
            get
            {
                return base.FieldByName(fld_address).Value as IPAddress;
            }

            set
            {
                base.FieldByName(fld_address).Value = value;
            }
        }

        public int Port
        {
            get
            {
                return (int)base.FieldByName(fld_port).Value;
            }

            set
            {
                base.FieldByName(fld_port).Value = value;
            }
        }

    }
}
