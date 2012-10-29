using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    var t = m_asmManager.GetPluginDetails();

                    if (t == null || t.Count == 0)
                    {
                        throw new InvalidOperationException("The plugin assembly contains no Distrib Plugins");
                    }

                    // Go through all the plugins in the assembly
                    foreach (var pluginType in t)
                    {
                        // Check that the plugin type actually implements the interface the export attribute says it does
                        if (m_asmManager.PluginTypeAdheresToStatedInterface(pluginType))
                        {
                            result.AddOkPlugin(pluginType);
                        }
                        else
                        {
                            result.AddBadPlugin(new BadDistribPluginDetails(pluginType, DistribPluginExlusionReason.NonAdherenceToInterface));
                        }
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



    public sealed class DistribPluginAssemblyInitialisationResult
    {
        private readonly WriteOnce<bool> m_bLocked = new WriteOnce<bool>(initialValue: false);
        private readonly object m_lock = new object();

        private List<DistribPluginDetails> m_lstOkPlugins = new List<DistribPluginDetails>();
        private List<BadDistribPluginDetails> m_lstBadPlugins = new List<BadDistribPluginDetails>();

        internal void AddOkPlugin(DistribPluginDetails pluginDetails)
        {
            lock (m_lock)
            {
                if (!m_bLocked)
                {
                    lock (m_lstOkPlugins)
                    {
                        m_lstOkPlugins.Add(pluginDetails);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Cannot add OK plugin, result is locked");
                }
            }
        }

        internal void AddBadPlugin(BadDistribPluginDetails badPluginDetails)
        {
            lock (m_lock)
            {
                if (!m_bLocked)
                {
                    lock (m_lstBadPlugins)
                    {
                        m_lstBadPlugins.Add(badPluginDetails);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Cannot add bad plugin, result is locked");
                }
            }
        }

        internal void LockResult()
        {
            lock (m_lock)
            {
                if (!m_bLocked)
                {
                    m_bLocked.Value = true;
                }
                else
                {
                    throw new InvalidOperationException("Cannot lock result; it is already locked");
                }
            }
        }

        private WriteOnce<IReadOnlyList<DistribPluginDetails>> m_okPluginsReadOnly = new WriteOnce<IReadOnlyList<DistribPluginDetails>>(null);
        public IReadOnlyList<DistribPluginDetails> OkPlugins
        {
            get
            {
                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        lock (m_lstOkPlugins)
                        {
                            return m_lstOkPlugins.AsReadOnly();
                        }
                    }
                    else
                    {
                        if (!m_okPluginsReadOnly.IsWritten)
                        {
                            m_okPluginsReadOnly.Value = m_lstOkPlugins.AsReadOnly();
                            m_lstOkPlugins = null;  // Destroy the original list
                        }

                        return m_okPluginsReadOnly.Value;
                    }
                }
            }
        }

        private WriteOnce<IReadOnlyList<BadDistribPluginDetails>> m_badPluginsReadOnly = new WriteOnce<IReadOnlyList<BadDistribPluginDetails>>(null);
        public IReadOnlyList<BadDistribPluginDetails> BadPlugins
        {
            get
            {
                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        lock (m_lstBadPlugins)
                        {
                            return m_lstBadPlugins.AsReadOnly();
                        }
                    }
                    else
                    {
                        if (!m_badPluginsReadOnly.IsWritten)
                        {
                            m_badPluginsReadOnly.Value = m_lstBadPlugins.AsReadOnly();
                            m_lstBadPlugins = null; // Destroy the original list.
                        }

                        return m_badPluginsReadOnly.Value;
                    }
                }
            }
        }
    }

    public sealed class BadDistribPluginDetails
    {
        private readonly DistribPluginDetails m_pluginDetails = null;
        private readonly DistribPluginExlusionReason m_reason = DistribPluginExlusionReason.Unknown;

        public BadDistribPluginDetails(DistribPluginDetails pluginDetails, DistribPluginExlusionReason exlusionReason)
        {
            m_pluginDetails = pluginDetails;
            m_reason = ExclusionReason;
        }

        public DistribPluginDetails PluginDetails
        {
            get { return m_pluginDetails; }
        }

        public DistribPluginExlusionReason ExclusionReason
        {
            get { return m_reason; }
        }
    }

    public enum DistribPluginExlusionReason
    {
        Unknown = 0,
        NonAdherenceToInterface,
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
