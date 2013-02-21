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
