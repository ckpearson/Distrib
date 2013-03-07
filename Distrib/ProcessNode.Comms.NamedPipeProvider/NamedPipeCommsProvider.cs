using Distrib.Nodes.Process;
using DistribApps.Comms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.Communication;

namespace ProcessNode.Comms.NamedPipeProvider
{
    [Export(typeof(ICommsProvider<IProcessNodeComms>))]
    public sealed class NamedPipeCommsProvider :
        ICommsProvider<IProcessNodeComms>
    {
        public Distrib.Communication.IIncomingCommsLink<IProcessNodeComms> CreateIncomingLink(CommsEndpointDetails endpoint)
        {
            if (endpoint is NamedPipeEndpointDetails == false)
            {
                throw new InvalidOperationException();
            }

            var epoint = (NamedPipeEndpointDetails)endpoint;

            try
            {
                return new NamedPipeIncomingCommsLink<IProcessNodeComms>(
                    new Distrib.Communication.NamedPipeEndpointDetails()
                    {
                        MachineName = ".",
                        PipeName = epoint.Pipe,
                    }, new XmlCommsMessageReaderWriter(new BinaryFormatterCommsMessageFormatter()), new DirectInvocationCommsMessageProcessor());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create incoming named pipe link", ex);
            }
        }

        public Distrib.Communication.IOutgoingCommsLink<IProcessNodeComms> CreateOutgoing(CommsEndpointDetails endpoint)
        {
            throw new NotImplementedException();
        }

        public string ProviderType
        {
            get { return "Named Pipe"; }
        }

        public CommsEndpointDetails GetEndpointDetailsItem()
        {
            return new NamedPipeEndpointDetails(this)
            {
                Machine = Environment.MachineName,
                Pipe = "ProcessNodePipe",
            };
        }
    }

    public sealed class NamedPipeEndpointDetails : 
        CommsEndpointDetails
    {
        public const string fld_machine = "Machine";
        public const string fld_pipe = "Pipe";

        public NamedPipeEndpointDetails(ICommsProvider provider)
            : base("Named Pipe", provider)
        {

        }

        protected override IEnumerable<CommsEndpointDetailsField> CreateFields()
        {
            return new[]
            {
                new CommsEndpointDetailsField(fld_machine, Environment.MachineName, false),
                new CommsEndpointDetailsField(fld_pipe, "ProcessNodePipe", true)
                {
                    ValidationFunc = (s) =>
                        {
                            var st = Convert.ToString(s);
                            if (string.IsNullOrEmpty(st))
                            {
                                return "A pipe name must be supplied";
                            }

                            return null;
                        },
                },
            };
        }

        public string Machine
        {
            get { return (string)base.FieldByName(fld_machine).Value; }
            set
            {
                base.FieldByName(fld_machine).Value = value;
            }
        }

        public string Pipe
        {
            get { return (string)base.FieldByName(fld_pipe).Value; }
            set
            {
                base.FieldByName(fld_pipe).Value = value;
            }
        }
    }
}
