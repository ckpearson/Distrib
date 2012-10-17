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

        public void Initialise()
        {
            try
            {
                lock (m_objLock)
                {
                    if (IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot initialise plugin assembly; it is already initialised");
                    }

                    m_adAssemblyAppDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + m_strAssemblyPath);

                    try
                    {
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
                        m_asmManager.LoadAssembly();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Manager failed to load assembly into AppDomain", ex);
                    }

                    var t = m_asmManager.GetPluginTypes();

                    if (t == null || t.Count == 0)
                    {
                        throw new InvalidOperationException("The plugin assembly contains no Distrib Plugins");
                    }

                    foreach (var pluginType in t)
                    {
                        
                    }

                    m_bIsInitialised = true;
                }
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
