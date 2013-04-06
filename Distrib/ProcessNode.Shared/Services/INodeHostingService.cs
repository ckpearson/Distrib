using Distrib.Nodes.Process;
using Distrib.Processes;
using DistribApps.Comms;
using DistribApps.Core.Processes.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        IManagedProcessNode Node { get; }

        IEnumerable<IHostProvider> HostProviders { get; }
    }

    public interface IManagedProcessNode
    {
        IProcessNode CoreNode { get; }

        ObservableCollection<IManagedProcessHost> Hosts { get; }

        void AddHost(IManagedProcessHost host);
    }

    public interface IManagedProcessHost
    {
        IProcessHost CoreHost { get; }

        IProcessMetadata Metadata { get; }
    }
}
