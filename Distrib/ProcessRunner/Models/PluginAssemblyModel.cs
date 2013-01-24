using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using System.Threading;

namespace ProcessRunner.Models
{
    public sealed class PluginAssemblyModel : ModelBase
    {
        private readonly string _assemblyPath;
        private IPluginAssembly _pluginAssembly;

        private IPluginAssemblyInitialisationResult _initResult;

        public PluginAssemblyModel(string path)
        {
            _assemblyPath = path;
        }

        public bool IsInitialised
        {
            get
            {
                _createAssemblyIfNeeded();
                return _pluginAssembly.IsInitialised;
            }
        }

        public string Path
        {
            get
            {
                _createAssemblyIfNeeded();
                return _pluginAssembly.AssemblyFilePath;
            }
        }

        private void _createAssemblyIfNeeded()
        {
            if (_pluginAssembly == null)
            {
                _pluginAssembly = DistribIOC.Kernel.Get<IPluginAssemblyFactory>().CreatePluginAssemblyFromPath(_assemblyPath);
            }
        }

        public void Initialise()
        {
            _createAssemblyIfNeeded();
            if (_pluginAssembly.IsInitialised)
            {
                return;
            }
            _initResult = _pluginAssembly.Initialise();
            onPropChange("IsInitialised");
        }

        public void Uninitialise()
        {

        }
    }
}
