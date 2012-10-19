﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    /// <summary>
    /// Holds the metadata attached to a plugin within the Distrib system.
    /// </summary>
    [Serializable()]
    public sealed class DistribPluginMetadata
    {
        private readonly string m_strName = "";
        private readonly string m_strDescription = "";
        private readonly double m_dVersion = 0.0;
        private readonly string m_strAuthor = "";

        private readonly Type m_typInterfaceType = null;

        /// <summary>
        /// Instantiates a new instance of <see cref="DistribPluginMetadata"/>
        /// </summary>
        /// <param name="interfaceType">The plugin interface type</param>
        /// <param name="name">The name of the plugin</param>
        /// <param name="description">The description of the plugin</param>
        /// <param name="version">The version of the plugin</param>
        /// <param name="author">The author of the plugin</param>
        public DistribPluginMetadata(Type interfaceType,
            string name,
            string description,
            double version,
            string author)
        {
            m_typInterfaceType = interfaceType;
            m_strName = name;
            m_strDescription = description;
            m_dVersion = version;
            m_strAuthor = author;
        }

        /// <summary>
        /// The interface <see cref="System.Type"/> that this plugin implements
        /// </summary>
        public Type InterfaceType
        {
            get { return m_typInterfaceType; }
        }

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public string Name
        {
            get { return m_strName; }
        }

        /// <summary>
        /// The description of the plugin
        /// </summary>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// The version of the plugin
        /// </summary>
        public double Version
        {
            get { return m_dVersion; }
        }

        /// <summary>
        /// The author of the plugin
        /// </summary>
        public string Author
        {
            get { return m_strAuthor; }
        }

        /// <summary>
        /// Creates a new <see cref="DistribPluginMetadata"/> from the details of a <see cref="DistribPluginAttribute"/> decorating
        /// a plugin class.
        /// </summary>
        /// <param name="attribute">The attribute to get the metadata from</param>
        /// <returns>The <see cref="DistribPluginMetadata"/> containing the details from the <see cref="DistribPluginAttribute"/></returns>
        public static DistribPluginMetadata FromPluginAttribute(DistribPluginAttribute attribute)
        {
            return new DistribPluginMetadata(
                attribute.InterfaceType,
                attribute.Name,
                attribute.Description,
                attribute.Version,
                attribute.Author);
        }
    }
}
