using Distrib.IOC;
using Distrib.IOC.Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistribApps.Core.Services
{
    [Export(typeof(IDistribAccessService))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.Shared)]
    public sealed class DistribAccessService :
        IDistribAccessService
    {
        private readonly IIOC _ioc;

        [ImportingConstructor]
        public DistribAccessService()
        {
            _ioc = new NinjectBootstrapper();
            _ioc.Start();
        }

        public IIOC DistribIOC
        {
            get { return _ioc; }
        }
    }

}
