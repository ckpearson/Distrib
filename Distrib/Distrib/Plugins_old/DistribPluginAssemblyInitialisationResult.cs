using Distrib.Plugins_old.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old
{
    /// <summary>
    /// Represents the result of plugin assembly initialisation
    /// </summary>
    /// <remarks>
    /// <para>
    /// Despite the apparent throw-away nature of the initialisation result, this result is important for usage of a plugin assembly.
    /// </para>
    /// <para>
    /// The result allows you to determine which plugins can be used and which can't, while this could be discovered without the result
    /// by interfacing with the <see cref="DistribPluginAssembly"/> directly, this result represents a handy self-contained means for
    /// working directly with the plugin details discovered during initialisation.
    /// </para>
    /// </remarks>
    public sealed class DistribPluginAssemblyInitialisationResult
    {
        private readonly WriteOnce<bool> m_bLocked = new WriteOnce<bool>(initialValue: false);
        private readonly object m_lock = new object();

        private List<PluginDetails> m_lstPlugins = new List<PluginDetails>();
        private readonly WriteOnce<IReadOnlyList<PluginDetails>> m_lstReadonlyPlugins = 
            new WriteOnce<IReadOnlyList<PluginDetails>>();

        /// <summary>
        /// Gets the list of plugins discovered during initialisation (good and bad)
        /// </summary>
        public IReadOnlyList<PluginDetails> Plugins
        {
            get
            {
                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        return m_lstPlugins.AsReadOnly();
                    }
                    else
                    {
                        if (!m_lstReadonlyPlugins.IsWritten)
                        {
                            m_lstReadonlyPlugins.Value = m_lstPlugins.AsReadOnly();
                            m_lstPlugins = null; // Destroy original list
                        }

                        return m_lstReadonlyPlugins.Value;
                    }
                }
            }
        }

        private readonly WriteOnce<IReadOnlyList<PluginDetails>> m_lstReadonlyGoodPlugins =
            new WriteOnce<IReadOnlyList<PluginDetails>>(null);

        /// <summary>
        /// Gets those plugins that were found during initialisation to be usable.
        /// </summary>
        public IReadOnlyList<PluginDetails> UsablePlugins
        {
            get
            {
                Func<IReadOnlyList<PluginDetails>> retr = 
                    () => Plugins.Where(p => p.IsUsable).ToList().AsReadOnly();

                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        return retr();
                    }
                    else
                    {
                        if (!m_lstReadonlyGoodPlugins.IsWritten)
                        {
                            m_lstReadonlyGoodPlugins.Value = retr();
                        }

                        return m_lstReadonlyGoodPlugins.Value;
                    }
                }
            }
        }

        private readonly WriteOnce<IReadOnlyList<PluginDetails>> m_lstReadonlyBadPlugins =
            new WriteOnce<IReadOnlyList<PluginDetails>>(null);

        /// <summary>
        /// Gets those plugins that were found during initialisation to be unusable and are excluded from use.
        /// </summary>
        public IReadOnlyList<PluginDetails> ExcludedPlugins
        {
            get
            {
                Func<IReadOnlyList<PluginDetails>> retr =
                    () => Plugins.Where(p => !p.IsUsable).ToList().AsReadOnly();

                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        return retr();
                    }
                    else
                    {
                        if (!m_lstReadonlyBadPlugins.IsWritten)
                        {
                            m_lstReadonlyBadPlugins.Value = retr();
                        }

                        return m_lstReadonlyBadPlugins.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets whether there are any usable plugins
        /// </summary>
        public bool HasUsablePlugins
        {
            get
            {
                lock (m_lock)
                {
                    return UsablePlugins != null && UsablePlugins.Count > 0; 
                }
            }
        }

        /// <summary>
        /// Gets whether there are any excluded plugins
        /// </summary>
        public bool HasExcludedPlugins
        {
            get
            {
                lock (m_lock)
                {
                    return ExcludedPlugins != null && ExcludedPlugins.Count > 0; 
                }
            }
        }

        /// <summary>
        /// Adds a plugin to the list of plugins
        /// </summary>
        /// <param name="pluginDetails">The details of the plugin</param>
        internal void AddPlugin(PluginDetails pluginDetails)
        {
            try
            {
                lock (m_lock)
                {
                    if (!m_bLocked)
                    {
                        // Make sure the usability of the plugin has been set

                        if (!pluginDetails.UsabilitySet)
                        {
                            throw new InvalidOperationException("Cannot add plugin; its usability hasn't been set");
                        }
                        else
                        {
                            lock (m_lstPlugins)
                            {
                                m_lstPlugins.Add(pluginDetails);
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot add plugin; result is locked");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add plugin to result", ex);
            }
        }

        /// <summary>
        /// Marks the result as locked and prevents any further modifications
        /// </summary>
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
    }
}
