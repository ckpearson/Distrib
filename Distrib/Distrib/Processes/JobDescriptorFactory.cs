using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Core job descriptor factory
    /// </summary>
    public sealed class JobDescriptorFactory : CrossAppDomainObject, IJobDescriptorFactory
    {
        private readonly IKernel _kernel;

        public JobDescriptorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IJobDescriptor Create(IJobDefinition definition)
        {
            return _kernel.Get<IJobDescriptor>(new[]
            {
                new ConstructorArgument("jobName", definition.Name),
                new ConstructorArgument("inputFields", definition.InputFields),
                new ConstructorArgument("outputFields", definition.OutputFields),
            });
        }
    }
}
