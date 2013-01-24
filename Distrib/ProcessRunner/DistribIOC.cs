using Distrib.IOC;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner
{
    public static class DistribIOC
    {
        private static IKernel _kernel;

        public static IKernel Kernel
        {
            get
            {
                if (_kernel == null)
                {
                    _kernel = _kernel = new StandardKernel(
                    typeof(PluginsNinjectModule).Assembly.GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Equals(typeof(NinjectModule)))
                    .Select(t => Activator.CreateInstance(t) as INinjectModule)
                    .ToArray());
                }

                return _kernel;
            }
        }
    }
}
