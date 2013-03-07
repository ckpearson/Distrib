using Distrib.Nodes.Process;
using DistribApps.Comms;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Comms.NamedPipeProvider
{
    [Export(typeof(ICommsProvider<IProcessNodeComms>))]
    public sealed class NamedPipeCommsProvider :
        ICommsProvider<IProcessNodeComms>
    {
        public Distrib.Communication.IIncomingCommsLink<IProcessNodeComms> CreateIncomingLink(CommsEndpointDetails endpoint)
        {
            throw new NotImplementedException();
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
            return new NamedPipeEndpointDetails()
            {

            };
        }
    }

    public sealed class NamedPipeEndpointDetails : 
        CommsEndpointDetails
    {
        public const string fld_machine = "Machine";
        public const string fld_pipe = "Pipe";

        public NamedPipeEndpointDetails()
            : base("Named Pipe")
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
    }
}
