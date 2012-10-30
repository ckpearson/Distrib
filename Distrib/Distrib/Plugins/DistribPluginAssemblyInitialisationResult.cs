using Distrib.Plugins.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
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

        private List<DistribPluginDetails> m_lstPlugins = new List<DistribPluginDetails>();
        private WriteOnce<IReadOnlyList<DistribPluginDetails>> m_lstReadonlyPlugins = new WriteOnce<IReadOnlyList<DistribPluginDetails>>();

        public IReadOnlyList<DistribPluginDetails> Plugins
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

        internal void AddPlugin(DistribPluginDetails pluginDetails)
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
