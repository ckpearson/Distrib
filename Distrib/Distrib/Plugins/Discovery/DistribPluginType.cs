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
        internal DistribPluginType(string name, string desc, Type interfaceType)
        {
            m_strPluginTypeName = name;
            m_strPluginTypeDescription = desc;
            m_typPluginInterfaceType = interfaceType;
        }
    }
}
