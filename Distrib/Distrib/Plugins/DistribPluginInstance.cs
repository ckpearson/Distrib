using Distrib.Plugins.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class DistribPluginInstance
    {
        private readonly DistribPluginDetails m_pluginDetails = null;
        private readonly DistribPluginAssembly m_parentAssembly = null;

        private AppDomain m_adAppDomain = null;
        private RemoteAppDomainBridge m_appDomainBridge = null;

        private object m_lock = new object();
        private bool m_bIsInitialised = false;

        private WriteOnce<object> m_instance = new WriteOnce<object>(null);

        internal DistribPluginInstance(DistribPluginDetails pluginDetails, DistribPluginAssembly parentAssembly)
        {
            m_pluginDetails = pluginDetails;
            m_parentAssembly = parentAssembly;
        }

        public T GetInstance<T>() where T : class
        {
            try
            {
                lock (m_lock)
                {
                    if (!typeof(T).IsInterface)
                    {
                        throw new InvalidOperationException("T must be an interface");
                    }

                    if (!typeof(T).Equals(m_pluginDetails.Metadata.InterfaceType))
                    {
                        throw new InvalidOperationException("T must be of same interface type for plugin metadata");
                    }

                    if (!IsInitialised())
                    {
                        Initialise();
                    }

                    if (!m_instance.IsWritten)
                    {
                        m_instance.Value = m_appDomainBridge.CreateInstance(
                            m_pluginDetails.PluginTypeName,
                            m_parentAssembly.AssemblyFilePath);
                    }

                    return (T)m_instance.Value;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get instance", ex);
            }
        }

        internal void Initialise()
        {
            try
            {
                lock (m_lock)
                {
                    if (IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot initialise; already initialised");
                    }

                    // Set up app domain
                    m_adAppDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + m_parentAssembly.AssemblyFilePath + "_" +
                        m_pluginDetails.PluginTypeName);

                    // Create remote bridge
                    m_appDomainBridge = (RemoteAppDomainBridge)m_adAppDomain.CreateInstanceAndUnwrap(
                        typeof(RemoteAppDomainBridge).Assembly.FullName,
                        typeof(RemoteAppDomainBridge).FullName);

                    // Load the plugin assembly into the AppDomain
                    m_appDomainBridge.LoadAssembly(m_parentAssembly.AssemblyFilePath);

                    m_instance = new WriteOnce<object>(null);

                    m_bIsInitialised = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise", ex);
            }
        }

        internal bool IsInitialised()
        {
            try
            {
                lock (m_lock)
                {
                    return m_bIsInitialised;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if initialised", ex);
            }
        }

        internal void Unitialise()
        {
            try
            {
                lock (m_lock)
                {
                    if (!IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot unitialise; not initialised");
                    }

                    // Destroy bridge
                    m_appDomainBridge = null;

                    // Unload AppDomain
                    AppDomain.Unload(m_adAppDomain);

                    // Cleanup
                    m_adAppDomain = null;
                    m_bIsInitialised = false;
                    m_instance = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to unitialise", ex);
            }
        }
    }
}
