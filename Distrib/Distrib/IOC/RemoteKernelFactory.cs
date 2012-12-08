using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Factory for creating a remote kernel
    /// </summary>
    public sealed class RemoteKernelFactory : IRemoteKernelFactory
    {
        private readonly IKernel _kernel;

        public RemoteKernelFactory(IKernel kernel)
        {
            _kernel = kernel;   // needed? (might be in future worthwhile having Ninject will just remove instance eventually)
        }

        public IRemoteKernel GetRemoteKernel(IKernel kernel)
        {
            // Return the remote kernel instance from the primary kernel
            return kernel.Get<IRemoteKernel>(
                new ConstructorArgument("getterFunc",
                    new Func<string, Tuple<string, object>[], object>(
                        (typeName, constructorArgs) =>
                        {
                            // Delegates kernel instance retrieval back to the kernel here
                            return kernel.Get(Type.GetType(typeName),
                                constructorArgs == null ? new IParameter[] { } :
                                    constructorArgs
                                        .Select(tup => new ConstructorArgument(tup.Item1, tup.Item2))
                                        .ToArray());
                        })));
        }
    }
}
