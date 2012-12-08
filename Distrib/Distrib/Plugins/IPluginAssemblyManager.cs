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
        object CreateInstanceFromPluginAssembly();

        IReadOnlyList<IPluginDescriptor> GetPluginDescriptors();

        bool PluginTypeAdheresToPluginInterface(IPluginDescriptor descriptor);
        bool PluginTypeImplementsCorePluginInterface(IPluginDescriptor descriptor);
        bool PluginTypeIsMarshalable(IPluginDescriptor descriptor);
    }
}
