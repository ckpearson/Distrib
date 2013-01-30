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
    }
}
