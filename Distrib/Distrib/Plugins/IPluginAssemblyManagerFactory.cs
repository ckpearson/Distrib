using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginAssemblyManagerFactory
    {
        IPluginAssemblyManager CreateManagerForAssembly(string assemblyPath);
        IPluginAssemblyManager CreateManagerForAssemblyInGivenDomain(string assemblyPath, AppDomain domain);
    }
}
