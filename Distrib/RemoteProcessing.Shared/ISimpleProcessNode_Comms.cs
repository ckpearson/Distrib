using Distrib.Communication;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteProcessing.Shared
{
    public interface ISimpleProcNode_Comms
    {
        IReadOnlyList<IJobDefinition> GetJobDefinitions();

        string SayHello(string name);
    }

    public sealed class SimpleProcNode_CommsProxy :
        Distrib.Communication.OutgoingCommsProxyBase<ISimpleProcNode_Comms>,
        ISimpleProcNode_Comms
    {
        public SimpleProcNode_CommsProxy(IOutgoingCommsLink<ISimpleProcNode_Comms> outgoingLink)
            : base(outgoingLink)
        {

        }

        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            return base.Link.InvokeMethod<IReadOnlyList<IJobDefinition>>(null);
        }


        public string SayHello(string name)
        {
            return base.Link.InvokeMethod<string>(new[] { name });
        }
    }
}
