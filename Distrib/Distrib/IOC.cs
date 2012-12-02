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

        //public T Get<T>(string name, string value)
        //{
        //    var result = Kernel.TryGet<T>(m => m.Has(name) &&
        //        (string.Equals(m.Get<string>(name), value,
        //            StringComparison.InvariantCultureIgnoreCase)));

        //    if (Equals(result, default(T))) throw new ApplicationException("Type not resolved");

        //    return result;
        //}

        public static void Inject(object item)
        {
            Kernel.Inject(item);
        }
    }

    internal sealed class UtilsModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IWriteOnce<>)).To(typeof(WriteOnce<>));
        }
    }
}
