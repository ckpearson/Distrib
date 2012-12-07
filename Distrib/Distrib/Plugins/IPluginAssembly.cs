using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginAssembly
    {
        
    }

    public interface IPluginAssemblyFactory
    {
        IPluginAssembly CreatePluginAssembly(string netAssemblyPath);
    }

    public sealed class PluginAssembly : IPluginAssembly
    {
        public PluginAssembly(string netAssemblyPath)
        {

        }
    }

    public sealed class PluginAssemblyFactory : IPluginAssemblyFactory
    {
        private IKernel _resolutionRoot;

        public PluginAssemblyFactory(IKernel resolutionRoot)
        {
            this._resolutionRoot = resolutionRoot;
        }

        public IPluginAssembly CreatePluginAssembly(string netAssemblyPath)
        {
            return _resolutionRoot.Get<IPluginAssembly>(new[]
            {
                new ConstructorArgument("netAssemblyPath", netAssemblyPath),
            });
        }
    }
}
