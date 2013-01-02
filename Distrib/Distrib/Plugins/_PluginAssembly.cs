using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class _PluginAssembly : _IPluginAssembly
    {
        private readonly string _assemblyLocation;

        private readonly object _lock = new object();

        private WriteOnce<IReadOnlyList<IPluginDescriptor>> _descriptors =
            new WriteOnce<IReadOnlyList<IPluginDescriptor>>(null);

        private readonly IPluginAssemblyManagerFactory _asmManagerFactory;

        private WriteOnce<bool> _isInitialised = new WriteOnce<bool>(false);

        private AppDomain _domain;
        private IPluginAssemblyManager _asmManager;

        public _PluginAssembly(IPluginAssemblyManagerFactory asmManagerFactory, string assemblyPath)
        {
            _asmManagerFactory = asmManagerFactory;
            _assemblyLocation = assemblyPath;
        }

        public string AssemblyLocation
        {
            get { return _assemblyLocation; }
        }

        public IEnumerable<IPluginDescriptor> PluginDescriptors
        {
            get
            {
                lock (_lock)
                {
                    // Perform any required initialisation
                    _doInit();

                    if (!_descriptors.IsWritten)
                    {
                       // Get the descriptors
                        _descriptors.Value = _asmManager.GetPluginDescriptors();
                    }

                    return _descriptors.Value;
                }
            }
        }

        private void _doInit()
        {
            lock (_lock)
            {
                if (_isInitialised.IsWritten)
                {
                    return;
                }

                try
                {
                    _domain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + _assemblyLocation);
                    _asmManager = _asmManagerFactory.CreateManagerForAssemblyInGivenDomain(
                        _assemblyLocation,
                        _domain);
                    _asmManager.LoadPluginAssemblyIntoDomain();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed to perform initialisation");
                }
            }
        }

        public IPluginInstance CreateInstance(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
