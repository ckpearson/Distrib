using Distrib.IOC;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class PluginAssemblyManager : MarshalByRefObject, IPluginAssemblyManager
    {
        private readonly IRemoteKernel _kernel;
        private readonly string _assemblyPath;

        private Assembly _assembly;

        private WeakReference<IReadOnlyList<IPluginDescriptor>> _pluginDescriptorsListReference;

        public PluginAssemblyManager(IRemoteKernel kernel, string assemblyPath)
        {
            _kernel = kernel;
            _assemblyPath = assemblyPath;
        }

        public void LoadPluginAssemblyIntoDomain()
        {
            try
            {
                _assembly = Assembly.LoadFile(_assemblyPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load assembly", ex);
            }
        }

        public object CreateInstanceFromPluginAssembly()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IPluginDescriptor> GetPluginDescriptors()
        {
            bool needToCreate = false;
            IReadOnlyList<IPluginDescriptor> readonlyDescriptorList = null;

            try
            {
                if (_pluginDescriptorsListReference == null)
                {
                    needToCreate = true;
                }
                else
                {
                    needToCreate = !_pluginDescriptorsListReference.TryGetTarget(out readonlyDescriptorList);
                }

                if (needToCreate)
                {
                    var types = _assembly
                        .GetTypes()
                        .Where(t => t.GetCustomAttribute<PluginAttribute>() != null)
                        .ToArray();

                    var lstTempDescriptors = new List<IPluginDescriptor>();

                    // Build up descriptor list

                    foreach (var type in types.Select(t => new { type = t, attr = t.GetCustomAttribute<PluginAttribute>() }))
                    {
                        // Create descriptor
                        var descriptor = _kernel.Get<IPluginDescriptorFactory>()
                            .GetDescriptor(type.type.FullName, 
                                _kernel.Get<IPluginMetadataFactory>()
                                    .CreateMetadataFromPluginAttribute(type.attr));

                        // See if the plugin has any additional metadata (this is only done via supplied metadata now)
                        descriptor.SetAdditionalMetadata(
                            type.attr.SuppliedMetadataObjects == null ? null :
                            type.attr.SuppliedMetadataObjects.Select(
                                mo => _kernel.Get<IPluginMetadataBundleFactory>()
                                    .CreateBundle(
                                        mo.MetadataInterfaceType,
                                        mo.ProvideMetadataInstance(),
                                        new ReadOnlyDictionary<string, object>(mo.ProvideMetadataKVPs()),
                                        mo.MetadataIdentity,
                                        mo.MetadataExistencePolicy)));
#warning New plugin system needs to discover additional metadata bundles
                    }
                }

                return readonlyDescriptorList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get plugin descriptors", ex);
            }
        }

        public bool PluginTypeAdheresToPluginInterface(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool PluginTypeImplementsCorePluginInterface(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public bool PluginTypeIsMarshalable(IPluginDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
