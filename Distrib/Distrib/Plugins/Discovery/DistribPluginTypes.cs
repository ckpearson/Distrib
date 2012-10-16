using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    public static class DistribPluginTypes
    {
        /// <summary>
        /// Plugin type for a processing node process
        /// </summary>
        public static readonly DistribPluginType DistribProcess =
            DistribPluginTypes.Create<IDistribProcess>("DistribProcess",
            "Represents a process for a distrib processing node");

        /// <summary>
        /// Gets a list of the different plugin types supported
        /// </summary>
        /// <returns>The list of <see cref="DistribPluginType"/></returns>
        public static List<DistribPluginType> GetPluginTypes()
        {
            List<DistribPluginType> types = new List<DistribPluginType>();

            try
            {
                return typeof(DistribPluginTypes).GetFields()
                    .Where(f => f.FieldType == typeof(DistribPluginType)).Select(f => f.GetValue(null) as DistribPluginType).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get plugin types", ex);
            }
        }

        public static bool PluginTypeExistsForInterface<T>() where T : class
        {
            return PluginTypeExistsForInterface(typeof(T));
        }

        public static bool PluginTypeExistsForInterface(Type type)
        {
            if (type == null) throw new ArgumentNullException("Type must be supplied");
            if (!type.IsInterface) throw new InvalidOperationException("Type supplied must be an interface type");

            try
            {
                return typeof(DistribPluginTypes).GetFields()
                    .Where(f => f.FieldType == typeof(DistribPluginType))
                    .Select(f => f.GetValue(null) as DistribPluginType)
                    .Any(t => t.m_typPluginInterfaceType == type);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if plugin type exists for interface type", ex);
            }
        }

        /// <summary>
        /// Creates a new plugin type
        /// </summary>
        /// <typeparam name="T">The interface <see cref="System.Type"/> the plugin utilises</typeparam>
        /// <param name="name">The name for this type of plugin</param>
        /// <param name="desc">The description for this type of plugin</param>
        /// <returns></returns>
        internal static DistribPluginType Create<T>(string name, string desc) where T : class
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name must not be null or empty");
            if (string.IsNullOrEmpty(desc)) throw new ArgumentException("Description must not be null or empty");

            try
            {
                var type = typeof(T);

                if (!type.IsInterface)
                {
                    throw new InvalidOperationException("Type must be an interface");
                }

                return new DistribPluginType(name, desc, type);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get plugin type", ex);
            }
        }

        public static bool IsTypeAValidPlugin<T>() where T : class
        {
            return IsTypeAValidPlugin(typeof(T));
        }

        public static bool IsTypeAValidPlugin(Type type)
        {
            try
            {
                // Check it's a class
                if (!type.IsClass)
                {
                    throw new InvalidOperationException("Type must be a class in order to be a valid plugin");
                }

                // Get all the interfaces implemented
                var interfaces = type.GetInterfaces();

                // Make sure there's at least one
                if (interfaces == null || interfaces.Length == 0)
                {
                    throw new InvalidOperationException("Type must implement at least one distrib plugin interface to be a valid plugin");
                }

                // Get the details attributes on the class
                var detailsAttributes = Attribute.GetCustomAttributes(type, typeof(DistribPluginDetailsAttribute))
                    .Cast<DistribPluginDetailsAttribute>();

                // Enumerate the details attributes.

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if type was a valid distrib plugin", ex);
            }

            return false;
        }
    }
}
