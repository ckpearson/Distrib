using Distrib.Plugins;
using Distrib.Separation;
using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHostFactory : IProcessHostFactory
    {
        private readonly IKernel _kernel;
        private readonly ISeparateInstanceCreatorFactory _instFactory;

        public ProcessHostFactory(IKernel kernel, ISeparateInstanceCreatorFactory instFactory)
        {
            _kernel = kernel;
            _instFactory = instFactory;
        }

        public IProcessHost CreateHostFromPlugin(IPluginDescriptor descriptor)
        {
            return _kernel.Get<IProcessHost>(new[]
            {
                new ConstructorArgument("descriptor", descriptor),
            });
        }

        public IProcessHost CreateHostFromPluginInDomain(IPluginDescriptor descriptor, AppDomain domain = null)
        {
            return _instFactory.CreateCreator().CreateInstance(
                CreateHostFromPlugin(descriptor).GetType(),
                new object[]
                {
                    descriptor,
                }) as IProcessHost;
        }
    }
}
