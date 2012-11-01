using Distrib.Plugins.Description;
using Distrib.Processes;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class DistribPluginAssembly
    {
        private readonly string m_strAssemblyPath = "";

        private AppDomain m_adAssemblyAppDomain = null;
        private DistribPluginAssemblyManager m_asmManager = null;

        private object m_objLock = new object();

        private bool m_bIsInitialised = false;

        private IReadOnlyList<DistribPluginDetails> m_lstPluginDetails = null;

        private DistribPluginAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("AssemblyPath must not be null or empty");

            m_strAssemblyPath = assemblyPath;
        }

        /// <summary>
        /// Performs initialisation and loads the assembly
        /// </summary>
        public DistribPluginAssemblyInitialisationResult Initialise()
        {
            DistribPluginAssemblyInitialisationResult result = new DistribPluginAssemblyInitialisationResult();

            try
            {
                lock (m_objLock)
                {
                    if (IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot initialise plugin assembly; it is already initialised");
                    }

                    // Set up the AppDomain
                    m_adAssemblyAppDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + m_strAssemblyPath);

                    try
                    {
                        // Create the assembly manager for the plugin assembly isolated in the AppDomain
                        m_asmManager = (DistribPluginAssemblyManager)m_adAssemblyAppDomain
                                        .CreateInstanceAndUnwrap(
                                        typeof(DistribPluginAssemblyManager).Assembly.FullName,
                                        typeof(DistribPluginAssemblyManager).FullName,
                                        true,
                                        System.Reflection.BindingFlags.CreateInstance,
                                        null,
                                        new object[] { m_strAssemblyPath },
                                        null,
                                        null);

                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create manager in AppDomain", ex);
                    }

                    if (m_asmManager == null)
                    {
                        throw new ApplicationException("Manager in AppDomain was null after creation");
                    }

                    try
                    {
                        // Load the assembly into the domain
                        m_asmManager.LoadAssembly();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Manager failed to load assembly into AppDomain", ex);
                    }

                    // Retrieve the details for all plugins present in the assembly
                    // At this point these are the details that should be utilised by the developer and the system
                    // for *actual* work as they are serialised through the AppDomain and as a result
                    // any modifications here are not represented in the manager (usability etc)
                    // When pushing down to the manager to perform low-level work, the type-name is used as the key
                    // and the plugin details data will simply be used from the version given to the manager.
                    m_lstPluginDetails = m_asmManager.GetPluginDetails();

                    if (m_lstPluginDetails == null || m_lstPluginDetails.Count == 0)
                    {
                        throw new InvalidOperationException("The plugin assembly contains no Distrib Plugins");
                    }

                    // Go through all the plugins in the assembly
                    foreach (var pluginType in m_lstPluginDetails)
                    {
                        // Check over the usability of the plugin and mark it accordingly
                        _CheckUsabilityOfPlugin(pluginType);

                        result.AddPlugin(pluginType);
                    }

                    m_bIsInitialised = true;
                }

                // Lock the result to ensure no modifications
                result.LockResult();
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin assembly", ex);
            }
        }

        public T CreatePluginInstance<T>(DistribPluginDetails details) where T : class
        {
            if (details == null) throw new ArgumentNullException("Plugin details must be supplied");
            if (!typeof(T).IsInterface) throw new InvalidOperationException("T must be an interface type");
            if (!details.Metadata.InterfaceType.Equals(typeof(T))) throw new InvalidOperationException("T must be of the plugin interface type");

            try
            {
                if (!details.IsUsable)
                {
                    throw new InvalidOperationException(string.Format("Plugin is marked as excluded because: {0}",
                        details.ExclusionReason.ToString()));
                }

                T o = (T)m_asmManager.CreateInstance(details.PluginTypeName);
                
                return o;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin instance", ex);
            }
        }

        private void _CheckUsabilityOfPlugin(DistribPluginDetails pluginType)
        {
            if (pluginType == null) throw new ArgumentNullException("Plugin Details must be supplied");

            try
            {
                // Check the plugin class implements the interface it claims to

                if (!m_asmManager.PluginTypeAdheresToStatedInterface(pluginType))
                {
                    pluginType.MarkAsUnusable(DistribPluginExlusionReason.NonAdherenceToInterface);
                    return;
                }

                // Got this far, so the plugin is classed as usable
                pluginType.MarkAsUsable();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check usability of plugin", ex);
            }
        }

        public void Uninitialise()
        {
            try
            {
                lock (m_objLock)
                {
                    if (!IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot unitialise plugin assembly; it isn't initialised");
                    }

                    m_asmManager = null;

                    try
                    {
                        AppDomain.Unload(m_adAssemblyAppDomain);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to unload plugin assembly AppDomain", ex);
                    }

                    m_adAssemblyAppDomain = null;

                    m_bIsInitialised = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to unitialise plugin assembly", ex);
            }
        }

        public bool IsInitialised()
        {
            try
            {
                lock (m_objLock)
                {
                    return m_bIsInitialised;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if assembly is already initialised", ex);
            }
        }

        public static DistribPluginAssembly CreateForAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("AssemblyPath must not be null or empty");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException("Plugin assembly file not found", assemblyPath);
                }

                return new DistribPluginAssembly(assemblyPath);
            }
            catch (Exception ex)
            {
                throw new FailedToLoadDistribPluginAssemblyException(assemblyPath,
                    "Failed to load plugin assembly", ex);
            }
        }
    }

    [Serializable]
    public class FailedToLoadDistribPluginAssemblyException : Exception
    {
        private readonly string m_strAssemblyPath = "";

        public FailedToLoadDistribPluginAssemblyException() { }
        public FailedToLoadDistribPluginAssemblyException(string assemblyPath, string message)
            : base(message)
        {
            m_strAssemblyPath = assemblyPath;
        }
        public FailedToLoadDistribPluginAssemblyException(string assemblyPath, string message, Exception inner)
            : base(message, inner)
        {
            m_strAssemblyPath = assemblyPath;
        }
        protected FailedToLoadDistribPluginAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
