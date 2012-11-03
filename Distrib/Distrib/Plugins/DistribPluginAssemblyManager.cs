using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Distrib.Plugins.Discovery;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Distrib.Utils;
using Distrib.Plugins.Description;
using Distrib.Processes;
using System.Runtime.Remoting;

namespace Distrib.Plugins
{
    internal sealed class DistribPluginAssemblyManager : MarshalByRefObject
    {
        private readonly string m_strPluginAssemblyPath = "";
        private Assembly m_asmPluginAssembly = null;

        private WeakReference<List<DistribPluginDetails>> m_lstPluginDetails = null;

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

        //public IDistribProcess CreateProcessPlugin(string typeName)
        //{
        //    var t = m_asmPluginAssembly.GetType(typeName);
        //    var o = (IDistribProcess)Activator.CreateInstance(t);

        //    return ((MarshalByRefObject)o);
        //}

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
        public ReadOnlyCollection<DistribPluginDetails> GetPluginDetails()
        {
            bool needToCreate = false;
            List<DistribPluginDetails> lstDetails = null;

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

                lstDetails = new List<DistribPluginDetails>();

                // Build up the details list
                foreach (var type in types.Select(t => new { type = t, attr = t.GetCustomAttribute<DistribPluginAttribute>() }))
                {
                    // Add entry to the list
                    lstDetails.Add(new DistribPluginDetails(type.type.FullName, DistribPluginMetadata.FromPluginAttribute(type.attr)));
                }

                if (m_lstPluginDetails == null)
                {
                    m_lstPluginDetails = new WeakReference<List<DistribPluginDetails>>(lstDetails);
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
        public bool PluginTypeAdheresToStatedInterface(DistribPluginDetails pluginDetails)
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
        /// Determines whether a given plugin can be marshaled
        /// </summary>
        /// <param name="pluginDetails">The plugin details</param>
        /// <returns><c>True</c> if it can, <c>False</c> otherwise</returns>
        public bool PluginTypeIsMarshalable(DistribPluginDetails pluginDetails)
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
