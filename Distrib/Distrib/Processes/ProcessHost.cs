using Distrib.IOC;
using Distrib.Plugins;
using Distrib.Separation;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    public sealed class ProcessHost : MarshalByRefObject, IProcessHost
    {
        private readonly IPluginDescriptor _descriptor;
        private readonly IPluginAssemblyFactory _assemblyFactory;

        private bool _isInitialised = false;

        private readonly object _lock = new object();

        private IPluginAssembly _pluginAssembly;

        private IPluginInstance _pluginInstance;

        private IProcess _processInstance;

        public ProcessHost([IOC(false)] IPluginDescriptor descriptor, [IOC(true)] IPluginAssemblyFactory assemblyFactory)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");
            if (assemblyFactory == null) throw new ArgumentNullException("Assembly factory must be supplied");

            try
            {
                if (!descriptor.IsUsable)
                {
                    throw new InvalidOperationException("Descriptor must be for a usable plugin");
                }

                if (!descriptor.Metadata.InterfaceType.Equals(typeof(IProcess)))
                {
                    throw new InvalidOperationException("Descriptor must be for a process plugin");
                }

                _descriptor = descriptor;
                _assemblyFactory = assemblyFactory;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to construct instance", ex);
            }
        }

        public void Initialise()
        {
            try
            {
                lock (_lock)
                {
                    if (IsInitialised)
                    {
                        throw new InvalidOperationException("Already initialised");
                    }

                    // Load the plugin .NET assembly itself into the AppDomain
                    System.Reflection.Assembly.LoadFrom(_descriptor.AssemblyPath);

                    // Hook into the assembly resolve so any types can be brought back over the instance
                    // domain
                    AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                        {
                            // Look in the current AppDomain for it, if it's not there (which it ought to be)
                            // then a null will result in an exception when resolving
                            return AppDomain.CurrentDomain.GetAssemblies()
                                .DefaultIfEmpty(null)
                                .SingleOrDefault(asm => asm.FullName == e.Name);
                        };

                    _pluginAssembly = _assemblyFactory.CreatePluginAssemblyFromPath(_descriptor.AssemblyPath);
                    var res = _pluginAssembly.Initialise();
                    if (!res.HasUsablePlugins)
                    {
                        throw new InvalidOperationException("Plugin assembly contains no usable plugins");
                    }

                    if (res.UsablePlugins.DefaultIfEmpty(null)
                        .SingleOrDefault(p => p.Match(_descriptor)) == null)
                    {
                        throw new InvalidOperationException("A matching plugin descriptor could not be found in the plugin assembly");
                    }

                    _pluginInstance = _pluginAssembly.CreatePluginInstance(_descriptor);

                    _pluginInstance.Initialise();

                    _processInstance = _pluginInstance.GetUnderlyingInstance<IProcess>();

                    _processInstance.InitProcess();

                    _isInitialised = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise", ex);
            }
        }

        public void Unitialise()
        {
            try
            {
                lock (_lock)
                {
                    if (!IsInitialised)
                    {
                        throw new InvalidOperationException("Not initialised");
                    }

                    _processInstance.UninitProcess();

                    if (_pluginAssembly != null && _pluginAssembly.IsInitialised)
                    {
                        _pluginAssembly.Unitialise();
                    }

                    _pluginAssembly = null;
                    _pluginInstance = null;
                    _processInstance = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to unitialise", ex);
            }
        }


        public bool IsInitialised
        {
            get
            {
                try
                {
                    lock (_lock)
                    {
                        return _isInitialised;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to determine if initialised", ex);
                }
            }
        }
    }
}
