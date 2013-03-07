using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Comms
{
    public interface ICommsProvider<TContract> where TContract : class
    {
        Distrib.Communication.IIncomingCommsLink<TContract> CreateIncomingLink(CommsEndpointDetails endpoint);
        Distrib.Communication.IOutgoingCommsLink<TContract> CreateOutgoing(CommsEndpointDetails endpoint);
        string ProviderType { get; }

        CommsEndpointDetails GetEndpointDetailsItem();
    }
}
