using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Processes;
using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Nodes.Process
{

    public interface IProcessNode
    {
        void CreateAndHost(Type processType);
        void CreateAndHost(IPluginDescriptor processPluginDescriptor);
    }

    public sealed class StandardProcessNode : IProcessNode
    {
        private readonly IProcessHostFactory _hostFactory;

        public StandardProcessNode(
            [IOC(true)] IProcessHostFactory hostFactory)
        {
            _hostFactory = hostFactory;
        }

        public void CreateAndHost(Type processType)
        {
            if (processType == null) throw Ex.ArgNull(() => processType);

            try
            {
                
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
    }

    public interface IProcessNodeFactory
    {
        IProcessNode Create();
    }

    public sealed class ProcessNodeFactory : IProcessNodeFactory
    {
        private readonly IIOC _ioc;

        public ProcessNodeFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IProcessNode Create()
        {
            return _ioc.Get<IProcessNode>();
        }
    }

    public sealed class TempProcessNodeRegistrar : IOCRegistrar
    {
        public override void PerformBindings()
        {
            BindSingleton<IProcessNodeFactory, ProcessNodeFactory>();
            Bind<IProcessNode, StandardProcessNode>();
        }
    }

}
