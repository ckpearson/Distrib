using Distrib.Separation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginAssemblyManager
    {
        void LoadPluginAssemblyIntoDomain();

        IReadOnlyList<IPluginDescriptor> GetPluginDescriptors();

        bool PluginTypeImplementsPromisedInterface(IPluginDescriptor descriptor);
        bool PluginTypeImplementsCorePluginInterface(IPluginDescriptor descriptor);
        bool PluginTypeIsMarshalable(IPluginDescriptor descriptor);
    }
}
