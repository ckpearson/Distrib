using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    public interface IRemoteDomainBridge
    {
        void LoadAssembly(string filePath);
        object CreateInstance(string typeName, string assemblyPath);
    }
}
