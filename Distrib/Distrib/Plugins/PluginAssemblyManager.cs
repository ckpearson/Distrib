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
    public sealed class PluginAssemblyManager : CrossAppDomainObject, IPluginAssemblyManager
    {
        private readonly string _assemblyPath;

        private readonly IPluginDescriptorFactory _pluginDescriptorFactory;
        private readonly IPluginMetadataFactory _pluginMetadataFactory;
        private readonly IPluginMetadataBundleFactory _pluginMetadataBundleFactory;

        private Assembly _assembly;

        private WeakReference<IReadOnlyList<IPluginDescriptor>> _pluginDescriptorsListReference;

        public PluginAssemblyManager(IPluginDescriptorFactory pluginDescriptorFactory,
            IPluginMetadataFactory pluginMetadataFactory,
            IPluginMetadataBundleFactory pluginMetadataBundleFactory,
            string assemblyPath)
        {
            _pluginDescriptorFactory = pluginDescriptorFactory;
            _pluginMetadataFactory = pluginMetadataFactory;
            _pluginMetadataBundleFactory = pluginMetadataBundleFactory;

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
                        var descriptor = _pluginDescriptorFactory
                            .GetDescriptor(type.type.FullName,
                                _pluginMetadataFactory
                                    .CreateMetadataFromPluginAttribute(type.attr),
                                    _assemblyPath);

                        // See if the plugin has any additional metadata (this is only done via supplied metadata now)
                        descriptor.SetAdditionalMetadata(
                            type.attr.SuppliedMetadataObjects == null ? null :
                            type.attr.SuppliedMetadataObjects.Select(
                                mo => _pluginMetadataBundleFactory
                                    .CreateBundleFromAdditionalMetadataObject(mo)));

                        lstTempDescriptors.Add(descriptor);
                    }

                    if (_pluginDescriptorsListReference == null)
                    {
                        _pluginDescriptorsListReference = new WeakReference<IReadOnlyList<IPluginDescriptor>>(lstTempDescriptors);
                    }
                    else
                    {
                        _pluginDescriptorsListReference.SetTarget(lstTempDescriptors);
                    }

                    readonlyDescriptorList = lstTempDescriptors.AsReadOnly();
                }

                return readonlyDescriptorList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get plugin descriptors", ex);
            }
        }

        public bool PluginTypeImplementsPromisedInterface(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                if (!_pluginExistsInAssembly(descriptor))
                {
                    throw new InvalidOperationException("A plugin matching the details supplied could not be found");
                }

                return _assembly.GetType(descriptor.PluginTypeName)
                    .GetInterface(descriptor.Metadata.InterfaceType.FullName) != null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin implements promised interface", ex);
            }
        }

        public bool PluginTypeImplementsCorePluginInterface(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                if (!_pluginExistsInAssembly(descriptor))
                {
                    throw new InvalidOperationException("A plugin matching the details supplied could not be found");
                }

                return _assembly.GetType(descriptor.PluginTypeName)
                    .GetInterface(typeof(IPlugin).FullName) != null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin implements core plugin interface", ex);
            }
        }

        public bool PluginTypeIsMarshalable(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                if (!_pluginExistsInAssembly(descriptor))
                {
                    throw new InvalidOperationException("A plugin matching the details supplied could not be found");
                }

                var t = _assembly.GetType(descriptor.PluginTypeName);

                return (t.BaseType != null && t.BaseType.Equals(typeof(CrossAppDomainObject)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin is marshalable", ex);
            }
        }

        private bool _pluginExistsInAssembly(IPluginDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException("Descriptor must be supplied");

            try
            {
                // While plugins do have identifiers that are intended to be unique, this is by no means
                // a concrete guarantee that a plugin is present.
                // Because the plugin type names are most likely to be unique these are used
                // as the basis of any presence tests.
                return (GetPluginDescriptors()
                    .DefaultIfEmpty(null)
                    .SingleOrDefault(d => d.PluginTypeName == descriptor.PluginTypeName) != null);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin exists within assembly", ex);
            }
        }
    }
}
