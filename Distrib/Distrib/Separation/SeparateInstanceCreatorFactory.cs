using Ninject;
using Ninject.Parameters;
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
            return _kernel.Get<ISeparateInstanceCreator>(new[]
            {
                new ConstructorArgument("iocHasType", new Func<Type, bool>((t) => 
                    {
                        var bindings = _kernel.GetBindings(t);
                        return bindings != null && bindings.Count() > 0;
                    })),

                new ConstructorArgument("iocGetInstance", new Func<Type, object>((t) => _kernel.Get(t))),
            });
        }
    }
}
