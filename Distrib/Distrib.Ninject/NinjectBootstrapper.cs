/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
    /// <summary>
    /// Provides a Distrib IOC bootstrapper for the Ninject dependency injection framework
    /// </summary>
    public class NinjectBootstrapper : Distrib.IOC.DistribBootstrapper
    {
        private IKernel _kernel = null;

        public NinjectBootstrapper() { }

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="ninjectKernel">The ninject kernel you want distrib to utilise</param>
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

        protected override void Rebind(Type serviceType, Type concreteType, bool singleton)
        {
            if (singleton)
            {
                _kernel.Rebind(serviceType).To(concreteType).InSingletonScope();
            }
            else
            {
                _kernel.Rebind(serviceType).To(concreteType);
            }
        }

        protected override void RebindToConstant(Type type, object instance)
        {
            _kernel.Rebind(type).ToConstant(instance);
        }
    }
}
