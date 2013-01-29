using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using Ninject.Parameters;

namespace Distrib.IOC.Ninject
{
    public class NinjectBootstrapper : Distrib.IOC.DistribBootstrapper
    {
        private IKernel _kernel;

        public NinjectBootstrapper()
        {

        }

        public NinjectBootstrapper(IKernel ninjectKernel)
        {
            _kernel = ninjectKernel;
        }

        protected override void Initialise()
        {
            if (_kernel == null)
            {
                _kernel = new StandardKernel(new INinjectModule[] { }); 
            }
        }

        protected override object GetInstance(Type serviceType, params IOC.IOCConstructorArgument[] args)
        {
            if (args == null || args.Length == 0)
            {
                return _kernel.Get(serviceType);
            }
            else
            {
                return _kernel.Get(serviceType, args.Select(a => new ConstructorArgument(a.Name, a.Value)).ToArray());
            }
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        protected override void Inject(object instance)
        {
            _kernel.Inject(instance);
        }

        protected override void Bind(Type serviceType, Type concreteType, bool singleton = false)
        {
            if (singleton)
            {
                _kernel.Bind(serviceType).To(concreteType).InSingletonScope();
            }
            else
            {
                _kernel.Bind(serviceType).To(concreteType);
            }
        }

        protected override void BindToConstant(Type serviceType, object instance)
        {
            _kernel.Bind(serviceType).ToConstant(instance);
        }

        protected override bool IsTypeRegistered(Type serviceType)
        {
            var bindings = _kernel.GetBindings(serviceType);
            return bindings != null && bindings.Count() > 0;
        }
    }
}
