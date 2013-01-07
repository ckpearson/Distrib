using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class PersistenceNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Persistence.IPersistenceDataBagFactory>().To<Persistence.PersistenceDataBagFactory>().InSingletonScope();
            Bind<Persistence.IPersistenceDataBag>().To<Persistence.PersistenceDataBag>();
        }
    }
}
