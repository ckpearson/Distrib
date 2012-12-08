using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public sealed class RemoteDomainBridge : MarshalByRefObject, IRemoteDomainBridge
    {
        public RemoteDomainBridge()
        {

        }

        public void LoadAssembly(string filePath)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(string typeName, string assemblyPath)
        {
            throw new NotImplementedException();
        }
    }
}
