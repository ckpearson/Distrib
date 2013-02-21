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
using Distrib.Communication;
using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes.PluginPowered;

namespace Distrib.Nodes.Process
{
    public sealed class Remote_ProcessNode : OutgoingCommsProxyBase<IProcessNodeComms>, IProcessNodeComms
    {
        public Remote_ProcessNode(IOutgoingCommsLink<IProcessNodeComms> link)
            : base(link)
        {
            
        }

        public int CurrentHostCount()
        {
            return Link.InvokeMethod<int>(null);
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitions()
        {
            return Link.InvokeMethod<IReadOnlyList<IJobDefinition>>(null);
        }


        public IReadOnlyList<IProcessMetadata> GetProcessesMetadata()
        {
            return Link.InvokeMethod<IReadOnlyList<IProcessMetadata>>(null);
        }


        public IReadOnlyList<IJobDefinition> GetJobDefinitionsForProcess(IProcessMetadata metadata)
        {
            return Link.InvokeMethod<IReadOnlyList<IJobDefinition>>(new object[] { metadata });
        }
    }

    public sealed class StandardProcessNode : IProcessNode, IProcessNodeComms
    {
        private readonly IProcessHostFactory _hostFactory;
        private readonly IProcessHostTypeService _hostTypeService;
        private readonly IIncomingCommsLink<IProcessNodeComms> _nodeIncoming;

        private readonly List<HostedProcess> _hosts = new List<HostedProcess>();

        public StandardProcessNode(
            [IOC(true)] IProcessHostFactory hostFactory,
            [IOC(true)] IProcessHostTypeService hostTypeService,
            [IOC(false)] IIncomingCommsLink<IProcessNodeComms> nodeIncoming)
        {
            if (hostFactory == null) throw Ex.ArgNull(() => hostFactory);
            if (nodeIncoming == null) throw Ex.ArgNull(() => nodeIncoming);

            if (nodeIncoming.IsListening) throw Ex.Arg(() => nodeIncoming,
                "The incoming link shouldn't be currently listening");

            _hostFactory = hostFactory;
            _hostTypeService = hostTypeService;
            _nodeIncoming = nodeIncoming;
            _nodeIncoming.StartListening(this);
        }

        public void CreateAndHost(Type processType)
        {
            if (processType == null) throw Ex.ArgNull(() => processType);
            if (!processType.IsClass) throw Ex.Arg(() => processType, "Type must be a class");

            try
            {
                lock (_hosts)
                {
                    var host = new TypePoweredHostedProcess(processType,
                        _hostTypeService.GetTypePoweredInterface(_hostFactory.CreateHostFromType(processType)));

                    host.Host.Initialise();

                    _hosts.Add(host);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create and host type-powered process host", ex);
            }
        }

        public void CreateAndHost(IPluginDescriptor processPluginDescriptor)
        {
            throw new NotImplementedException();
        }

        int IProcessNodeComms.CurrentHostCount()
        {
            lock (_hosts)
            {
                return _hosts.Count;
            }
        }


        IReadOnlyList<IJobDefinition> IProcessNodeComms.GetJobDefinitions()
        {
            var lst = new List<IJobDefinition>();

            lock (_hosts)
            {
                foreach (var host in _hosts.Where(h => h.Host != null && h.Host.IsInitialised)
                    .Select(h => h.Host))
                {
                    foreach (var jd in host.JobDefinitions)
                    {
                        if (!lst.Any(j => j.Match(jd)))
                        {
                            lst.Add(jd.ToFlattened());
                        }
                    }
                }
            }

            return lst.AsReadOnly();
        }


        IReadOnlyList<IProcessMetadata> IProcessNodeComms.GetProcessesMetadata()
        {
            var lst = new List<IProcessMetadata>();

            lock (_hosts)
            {
                foreach (var host in _hosts.Where(h => h.Host != null && h.Host.IsInitialised)
                    .Select(h => h.Host))
                {
                    lst.Add(host.Metadata);
                }
            }

            return lst.AsReadOnly();
        }


        IReadOnlyList<IJobDefinition> IProcessNodeComms.GetJobDefinitionsForProcess(IProcessMetadata metadata)
        {
            lock (_hosts)
            {
                var host = _hosts.Where(h => h.Host.Metadata.Match(metadata));
            }

            return null;
        }
    }

    public abstract class HostedProcess
    {
        private SystemPowerType _type;
        private IProcessHost _host;

        public HostedProcess(SystemPowerType type, IProcessHost host)
        {
            _type = type;
            _host = host;
        }

        SystemPowerType PowerType { get { return _type; } }

        public IProcessHost Host { get { return _host; } }
    }

    public sealed class TypePoweredHostedProcess : HostedProcess
    {
        private readonly Type _type;

        public TypePoweredHostedProcess(Type type, ITypePoweredProcessHost host)
            : base(SystemPowerType.Type, host)
        {
            _type = type;
        }

        public Type Type { get { return _type; } }
        public new ITypePoweredProcessHost Host { get { return (ITypePoweredProcessHost)base.Host; } }
    }
}
