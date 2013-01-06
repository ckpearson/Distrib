using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class SeparationNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Separation.IRemoteDomainBridgeFactory>().To<Separation.RemoteDomainBridgeFactory>().InSingletonScope();
            Bind<Separation.IRemoteDomainBridge>().To<Separation.RemoteDomainBridge>();

            Bind<Separation.ISeparateInstanceCreatorFactory>().To<Separation.SeparateInstanceCreatorFactory>().InSingletonScope();
            Bind<Separation.ISeparateInstanceCreator>().To<Separation.SeparateInstanceCreator>();
        }
    }
}
