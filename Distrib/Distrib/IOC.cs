using Distrib.Utils;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib
{
    /// <summary>
    /// IOC service (Ninject) used by Distrib
    /// </summary>
    internal static class IOC
    {
        public static readonly IKernel Kernel;

        static IOC()
        {
            Kernel = new StandardKernel(new INinjectModule[]
                {
                    
                });
        }

        public static object Get(Type type)
        {
            return Kernel.Get(type);
        }

        public static object Get(Type type, params IParameter[] parameters)
        {
            return Kernel.Get(type, parameters);
        }

        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        public static T Get<T>(params IParameter[] parameters)
        {
            return Kernel.Get<T>(parameters);
        }

        public static void Inject(object item)
        {
            Kernel.Inject(item);
        }
    }
}
