﻿using Distrib.IOC;
using Ninject;
using System;
using System.Collections.Generic;
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
