using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distrib.IOC;
using Ninject.Parameters;

namespace Distrib.Processes
{
    public sealed class JobFactory : CrossAppDomainObject, IJobFactory
    {
        private readonly IKernel _kernel;
        
        public JobFactory([IOC(true)] IKernel kernel)
        {
            _kernel = kernel;
        }

        public IJob CreateJob(IJobInputTracker inputTracker, IJobOutputTracker outputTracker, IJobDefinition jobDefinition)
        {
            return _kernel.Get<IJob>(new ConstructorArgument[]
            {
                new ConstructorArgument("inputTracker", inputTracker),
                new ConstructorArgument("outputTracker", outputTracker),
                new ConstructorArgument("jobDefinition", jobDefinition),
            });
        }
    }
}
