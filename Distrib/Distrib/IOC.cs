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
                    new UtilsModule(),
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

    /// <summary>
    /// Ninject module for providing bindings for Util classes
    /// </summary>
    internal sealed class UtilsModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IWriteOnce<bool>>().ToConstructor<WriteOnce<bool>>((a) => new WriteOnce<bool>(false));

            Bind(typeof(IWriteOnce<>)).To(typeof(WriteOnce<>));
        }
    }
}
