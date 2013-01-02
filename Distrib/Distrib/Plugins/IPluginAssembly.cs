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
        IPluginAssemblyInitialisationResult Initialise();
        void Unitialise();
        bool IsInitialised { get; }

        string AssemblyFilePath { get; }

        IPluginInstance CreatePluginInstance(IPluginDescriptor descriptor);
    }

    public interface _IPluginAssembly
    {
        string AssemblyLocation { get; }
        IEnumerable<IPluginDescriptor> PluginDescriptors { get; }

        IPluginInstance CreateInstance(IPluginDescriptor descriptor);
    }
}
