using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public sealed class RemoteDomainBridge : CrossAppDomainObject, IRemoteDomainBridge
    {
        public RemoteDomainBridge()
        {

        }

        public void LoadAssembly(string filePath)
        {
            Assembly.LoadFrom(filePath);
        }

        public object CreateInstance(string typeName, object[] args)
        {
            return Activator.CreateInstance(Type.GetType(typeName), args);
        }
    }
}
