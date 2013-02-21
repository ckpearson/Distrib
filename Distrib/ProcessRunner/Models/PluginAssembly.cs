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
using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Models
{
    public sealed class PluginAssembly
    {
        private readonly string _path;
        private IPluginAssembly _pluginAssembly;
        private IPluginAssemblyFactory _pluginAssemblyFactory;

        private IPluginAssemblyInitialisationResult _initResult;

        public PluginAssembly(string path, IPluginAssemblyFactory assemblyFactory)
        {
            _path = path;
            _pluginAssemblyFactory = assemblyFactory;
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public void Init()
        {
            if (_pluginAssembly == null)
            {
                _pluginAssembly = _pluginAssemblyFactory.CreatePluginAssemblyFromPath(_path);
            }

            _initResult = _pluginAssembly.Initialise();
        }

        public void Uninit()
        {
            if (_pluginAssembly == null)
            {
                throw new InvalidOperationException();
            }

            _pluginAssembly.Unitialise();
        }

        public bool Initialised
        {
            get
            {
                return _pluginAssembly != null ? _pluginAssembly.IsInitialised : false;
            }
        }

        private IReadOnlyList<DistribPlugin> _plugins = null;
        public IReadOnlyList<DistribPlugin> Plugins
        {
            get
            {
                if (!Initialised)
                {
                    _plugins = null;
                    return null;
                }

                if (_plugins == null)
                {
                    _plugins = _initResult.Plugins.Select(p => new DistribPlugin(p)).ToList().AsReadOnly();
                }

                return _plugins;
            }
        }
    }
}
