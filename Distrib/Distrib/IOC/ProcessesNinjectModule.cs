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

            Bind<Distrib.Processes.IJobFactory>()
                .To<Distrib.Processes.JobFactory>().InSingletonScope();

            Bind<Distrib.Processes.IJob>()
                .To<Distrib.Processes.StandardProcessJob>();

            Bind<Distrib.Processes.IJobDescriptorFactory>()
                .To<Distrib.Processes.JobDescriptorFactory>().InSingletonScope();

            Bind<Distrib.Processes.IJobDescriptor>()
                .To<Distrib.Processes.JobDescriptor>();
        }
    }
}
