using Ninject.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Remote kernel proxy that exposes a standard Kernel interface but delegates across through a func
    /// </summary>
    public sealed class RemoteKernel : MarshalByRefObject, IRemoteKernel
    {
        private Func<string, Tuple<string, object>[], object> _getterFunc;

        public RemoteKernel(Func<string, Tuple<string, object>[], object> getterFunc)
        {
            _getterFunc = getterFunc;
        }

        public object Get(Type type, params Tuple<string, object>[] args)
        {
            return _getterFunc(type.FullName, args);
        }

        public T Get<T>(params Tuple<string, object>[] args)
        {
            return (T)_getterFunc(typeof(T).FullName, args);
        }
    }
}
