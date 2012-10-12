using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// Represents a plugin assembly for consumption by Distrib
    /// </summary>
    /// <remarks>
    /// <para>
    /// A plugin is the highest-level form of extensibility for Distrib; a plugin fundamentally comprises
    /// a DLL containing one or more plugin types.
    /// </para>
    /// <para>
    /// A plugin is loaded within a supplied AppDomain, with <see cref="Initialise"/> loading
    /// the assembly into the AppDomain and <see cref="Uninitialise"/> unloading that AppDomain.
    /// </para>
    /// </remarks>
    public sealed class DistribPlugin : IDisposable
    {
        private readonly AppDomain m_adPluginAppDomain = null;
        private PluginInitialiser m_pluginInitialiser = null;

        private readonly string m_strPluginAssemblyPath = string.Empty;

        private bool m_bIsInitialised = false;
        private object m_objLock = new object();

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="PluginDomain">The AppDomain the plugin assembly is to be loaded into</param>
        /// <param name="PluginAssemblyPath">The path to the assembly this plugin is for</param>
        public DistribPlugin(AppDomain PluginDomain, string PluginAssemblyPath)
        {
            if (PluginDomain == null) throw new ArgumentNullException("Plugin Domain must be supplied");
            if (string.IsNullOrEmpty(PluginAssemblyPath)) throw new ArgumentException("Plugin assembly path must not be null or empty");

            m_adPluginAppDomain = PluginDomain;
            m_strPluginAssemblyPath = PluginAssemblyPath;
        }

        /// <summary>
        /// Initialises the plugin by loading the assembly into the domain
        /// </summary>
        public void Initialise()
        {
            try
            {
                lock (m_objLock)
                {
                    if (m_bIsInitialised)
                    {
                        throw new InvalidOperationException("Plugin is already initialised");
                    }
                    else
                    {
                        try
                        {
                            if (m_pluginInitialiser == null)
                            {
                                m_pluginInitialiser = (PluginInitialiser)m_adPluginAppDomain.CreateInstanceAndUnwrap(
                                    typeof(PluginInitialiser).Assembly.FullName,
                                    typeof(PluginInitialiser).FullName);
                            }

                            m_pluginInitialiser.LoadAssemblyIntoPlugin(m_strPluginAssemblyPath);
                            m_bIsInitialised = true;
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Failed to load assembly for initialisation", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin", ex);
            }
        }

        /// <summary>
        /// Unitialises the plugin by unloading the domain for the plugin
        /// </summary>
        public void Uninitialise()
        {
            try
            {
                lock (m_objLock)
                {
                    if (!m_bIsInitialised)
                    {
                        throw new InvalidOperationException("Plugin isn't currently initialised");
                    }
                    else
                    {
                        try
                        {
                            if (!m_adPluginAppDomain.IsDefaultAppDomain())
                            {
                                AppDomain.Unload(m_adPluginAppDomain);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Failed to unload plugin domain", ex);
                        }

                        m_bIsInitialised = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to uninitialise plugin", ex);
            }
        }

        public void Dispose()
        {
            lock (m_objLock)
            {
                if (m_bIsInitialised)
                {
                    Uninitialise();
                }
            }
        }
    }
}
