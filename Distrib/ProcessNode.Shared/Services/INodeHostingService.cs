using Distrib.Nodes.Process;
using DistribApps.Comms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNode.Shared.Services
{
    public interface INodeHostingService
    {
        bool IsListening { get; }
        void StartListening(ICommsProvider<IProcessNodeComms> provider, CommsEndpointDetails endpoint);
        void StopListening();

        CommsEndpointDetails CurrentEndpoint { get; }
    }
}
