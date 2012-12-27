using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    public sealed class ProcessesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Distrib.Processes.IProcessHostFactory>()
                .To<Distrib.Processes.ProcessHostFactory>().InSingletonScope();

            Bind<Distrib.Processes.IProcessHost>()
                .To<Distrib.Processes.ProcessHost>();
        }
    }
}
