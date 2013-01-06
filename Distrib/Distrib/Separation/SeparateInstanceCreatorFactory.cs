using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public sealed class SeparateInstanceCreatorFactory : ISeparateInstanceCreatorFactory
    {
        private IKernel _kernel;

        public SeparateInstanceCreatorFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ISeparateInstanceCreator CreateCreator()
        {
            return _kernel.Get<ISeparateInstanceCreator>();
        }
    }
}
