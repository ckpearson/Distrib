/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
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
    }
}
