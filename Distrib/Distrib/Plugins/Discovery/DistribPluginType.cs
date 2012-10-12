using Distrib.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    /// <summary>
    /// Represents a type of plugin for the distrib system
    /// </summary>
    public sealed class DistribPluginType
    {
        public readonly string m_strPluginTypeName = "";
        public readonly string m_strPluginTypeDescription = "";
        public readonly Type m_typPluginInterfaceType = null;

        /// <summary>
        /// Instantiates a new instance of <see cref="DistribPluginType"/>
        /// </summary>
        /// <param name="name">The name of this type of plugin</param>
        /// <param name="desc">The description for this type of plugin</param>
        /// <param name="interfaceType">The interface <see cref="System.Type"/> this plugin utilises</param>
        private DistribPluginType(string name, string desc, Type interfaceType)
        {
            m_strPluginTypeName = name;
            m_strPluginTypeDescription = desc;
            m_typPluginInterfaceType = interfaceType;
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
    }


}
