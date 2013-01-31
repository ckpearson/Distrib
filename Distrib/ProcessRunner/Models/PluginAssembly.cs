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
