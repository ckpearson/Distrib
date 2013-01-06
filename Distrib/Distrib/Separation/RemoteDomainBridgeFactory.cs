using Ninject;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public sealed class RemoteDomainBridgeFactory : IRemoteDomainBridgeFactory
    {
        private readonly IKernel _kernel;

        public RemoteDomainBridgeFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IRemoteDomainBridge ForAppDomain(AppDomain domain)
        {
            var t = _kernel.Get<IRemoteDomainBridge>().GetType();

            return (IRemoteDomainBridge)domain.CreateInstanceAndUnwrap(
                t.Assembly.FullName,
                t.FullName);
        }
    }
}
