using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginAssemblyInitialisationResult
    {
        IReadOnlyList<IPluginDescriptor> Plugins { get; }
        IReadOnlyList<IPluginDescriptor> UsablePlugins { get; }
        IReadOnlyList<IPluginDescriptor> ExcludedPlugins { get; }

        bool HasUsablePlugins { get; }
        bool HasExcludedPlugins { get; }
    }
}
