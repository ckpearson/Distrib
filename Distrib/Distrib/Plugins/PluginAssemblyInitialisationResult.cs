/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
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
