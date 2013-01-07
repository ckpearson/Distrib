using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    public sealed class PluginAssemblyInitialisationResult : IPluginAssemblyInitialisationResult
    {
        private readonly IReadOnlyList<IPluginDescriptor> _plugins = null;
        private readonly WriteOnce<IReadOnlyList<IPluginDescriptor>> _usablePlugins =
            new WriteOnce<IReadOnlyList<IPluginDescriptor>>(null);
        private readonly WriteOnce<IReadOnlyList<IPluginDescriptor>> _excludedPlugins =
            new WriteOnce<IReadOnlyList<IPluginDescriptor>>(null);

        public PluginAssemblyInitialisationResult(IReadOnlyList<IPluginDescriptor> descriptorList)
        {
            _plugins = descriptorList;
        }

        public IReadOnlyList<IPluginDescriptor> Plugins
        {
            get { return _plugins; }
        }

        public IReadOnlyList<IPluginDescriptor> UsablePlugins
        {
            get
            {
                lock (_usablePlugins)
                {
                    if (!_usablePlugins.IsWritten)
                    {
                        _usablePlugins.Value = _plugins.Where(p => p.IsUsable).ToList().AsReadOnly();
                    }

                    return _usablePlugins.Value; 
                }
            }
        }

        
        public IReadOnlyList<IPluginDescriptor> ExcludedPlugins
        {
            get
            {
                lock (_excludedPlugins)
                {
                    if (!_excludedPlugins.IsWritten)
                    {
                        _excludedPlugins.Value = _plugins.Where(p => !p.IsUsable).ToList().AsReadOnly();
                    }

                    return _excludedPlugins.Value; 
                }
            }
        }

        public bool HasUsablePlugins
        {
            get
            {
                lock (_usablePlugins)
                {
                    return UsablePlugins != null && UsablePlugins.Count > 0;
                }
            }
        }

        public bool HasExcludedPlugins
        {
            get
            {
                lock (_excludedPlugins)
                {
                    return ExcludedPlugins != null && ExcludedPlugins.Count > 0;
                }
            }
        }
    }
}
