using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Distrib.Plugins_old.Discovery;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Distrib.Utils;
using Distrib.Plugins_old.Description;
using Distrib.Processes;
using System.Runtime.Remoting;
using Distrib.Plugins_old.Discovery.Metadata;

namespace Distrib.Plugins_old
{
    internal sealed class DistribPluginAssemblyManager : MarshalByRefObject
    {
        private readonly string m_strPluginAssemblyPath = "";
        private Assembly m_asmPluginAssembly = null;

        private WeakReference<List<PluginDetails>> m_lstPluginDetails = null;

        public DistribPluginAssemblyManager(string assemblyPath)
        {
            m_strPluginAssemblyPath = assemblyPath;
        }

        public void LoadAssembly()
        {
            try
            {
                m_asmPluginAssembly = Assembly.LoadFile(m_strPluginAssemblyPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load assembly", ex);
            }
        }

        public object CreateInstance(string typeName)
        {
            return Activator.CreateInstance(m_asmPluginAssembly.GetType(typeName));
        }

        /// <summary>
        /// Retrieves the details of all Distrib plugins within the assembly
        /// </summary>
        /// <returns>A <see cref="ReadOnlyCollection"/> containing the details of the plugins</returns>
        /// <remarks>
        /// <para>
        /// Given that the types present within an assembly are finite at time of post-compilation this data set can be cached and is.
        /// </para>
        /// <para>
        /// These details are typically only constructed once and then the cached version is retrieved, occasionally they may need to be
        /// re-constructed but calling this method should be considered safe enough to use for all queries for the plugin details.
        /// </para>
        /// </remarks>
        public ReadOnlyCollection<PluginDetails> GetPluginDetails()
        {
            bool needToCreate = false;
            List<PluginDetails> lstDetails = null;

            if (m_lstPluginDetails == null)
            {
                needToCreate = true;
            }
            else
            {
                // Need to create if can't get the list from the weak reference
                needToCreate = !m_lstPluginDetails.TryGetTarget(out lstDetails);
            }

            if (needToCreate)
            {
                // Get all of the types decorated with the plugin attribute
                var types = m_asmPluginAssembly
                            .GetTypes()
                            .Where(t => t.GetCustomAttribute<DistribPluginAttribute>() != null)
                            .ToArray();

                lstDetails = new List<PluginDetails>();

                // Build up the details list
                foreach (var type in types.Select(t => new { type = t, attr = t.GetCustomAttribute<DistribPluginAttribute>() }))
                {
                    // Create the plugin details
                    var pluginDetails = new PluginDetails(type.type.FullName, DistribPluginMetadata.FromPluginAttribute(type.attr));

                    // See if the plugin class has any additional plugin-specific metadata to carry across
                    var additMetadataDecorated = type.type.GetCustomAttributes<DistribPluginAdditionalMetadataAttribute>().ToList();
                    var additMetadataSupplied = type.attr.SuppliedAdditionalMetadata;

                    // The bundles need to comprise both of those provided by decoration and by explicit supply
                    // concatenate the bundles provided c
                    pluginDetails.SetAdditionalMetadata(
                        ((additMetadataDecorated == null) ? new List<IPluginAdditionalMetadataBundle>()
                            : additMetadataDecorated.Select(m => m.ToMetadataBundle()))
                        .Concat(
                            (additMetadataSupplied == null) ? new List<IPluginAdditionalMetadataBundle>()
                                : additMetadataSupplied));

                    // Add details to the list
                    lstDetails.Add(pluginDetails);
                }

                if (m_lstPluginDetails == null)
                {
                    m_lstPluginDetails = new WeakReference<List<PluginDetails>>(lstDetails);
                }
                else
                {
                    m_lstPluginDetails.SetTarget(lstDetails);
                } 
            }

            return lstDetails.AsReadOnly();
        }

        /// <summary>
        /// Determines whether a given plugin actually implements the interface its metadata claims
        /// </summary>
        /// <param name="pluginDetails">The details for the plugin to check</param>
        /// <returns><c>True</c> if the plugin type implements the interface, <c>False</c> otherwise</returns>
        public bool PluginTypeAdheresToStatedInterface(PluginDetails pluginDetails)
        {
            if (pluginDetails == null) throw new ArgumentNullException("Plugin details must be supplied");

            try
            {
                // Check to make sure a plugin with the same type name actually exists
                if (GetPluginDetails()
                        .DefaultIfEmpty(null)
                        .SingleOrDefault(d => d.PluginTypeName == pluginDetails.PluginTypeName) == null)
                {
                    throw new InvalidOperationException("A plugin type with the supplied details does not exist in the plugin assembly");
                }

                // Return whether the plugin type implements the stated interface
                return m_asmPluginAssembly.GetType(pluginDetails.PluginTypeName)
                    .GetInterface(pluginDetails.Metadata.InterfaceType.FullName) != null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin implements stated interface", ex);
            }
        }

        /// <summary>
        /// Determines whether a given plugin implements the core IDistribPlugin interface
        /// </summary>
        /// <param name="pluginDetails">The details of the plugin to check</param>
        /// <returns><c>True</c> if the plugin type implements the interface, <c>False</c> otherwise</returns>
        public bool PluginTypeImplementsDistribPluginInterface(PluginDetails pluginDetails)
        {
            if (pluginDetails == null) throw new ArgumentNullException("Plugin details must be supplied");

            try
            {
                // Check to make sure a plugin with the same type name actually exists
                if (GetPluginDetails()
                        .DefaultIfEmpty(null)
                        .SingleOrDefault(d => d.PluginTypeName == pluginDetails.PluginTypeName) == null)
                {
                    throw new InvalidOperationException("A plugin type with the supplied details does not exist in the plugin assembly");
                }

                // Return whether the plugin type implements the IDistribPlugin interface
                return m_asmPluginAssembly.GetType(pluginDetails.PluginTypeName)
                    .GetInterface(typeof(IDistribPlugin).FullName) != null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin implements distrib plugin interface", ex);
            }
        }

        /// <summary>
        /// Determines whether a given plugin can be marshaled
        /// </summary>
        /// <param name="pluginDetails">The plugin details</param>
        /// <returns><c>True</c> if it can, <c>False</c> otherwise</returns>
        public bool PluginTypeIsMarshalable(PluginDetails pluginDetails)
        {
            if (pluginDetails == null) throw new ArgumentNullException("Plugin details must be supplied");

            try
            {
                if (GetPluginDetails()
                    .DefaultIfEmpty(null)
                    .SingleOrDefault(d => d.PluginTypeName == pluginDetails.PluginTypeName) == null)
                {
                    throw new InvalidOperationException("A plugin type with the supplied details does not exist in the plugin assembly");
                }

                var t = m_asmPluginAssembly.GetType(pluginDetails.PluginTypeName);

                return (t.BaseType != null && t.BaseType.Equals(typeof(MarshalByRefObject)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin type is marshalable", ex);
            }
        }
    }


}
