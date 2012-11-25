using Distrib.Processes;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    /// <summary>
    /// Provides a base attribute for denoting a plugin export
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class DistribPluginAttribute : Attribute
    {
        private readonly string m_strName = "";
        private readonly string m_strDescription = "";
        private readonly double m_dVersion = 0.0;
        private readonly string m_strAuthor = "";
        private readonly string m_strIdentifier = "";

        private readonly Type m_typInterfaceType = null;

        private readonly WriteOnce<Type> m_typControllerType = new WriteOnce<Type>(null);

        private readonly WriteOnce<IReadOnlyList<IDistribPluginAdditionalMetadataBundle>> m_lstSuppliedAdditionalMetadata =
            new WriteOnce<IReadOnlyList<IDistribPluginAdditionalMetadataBundle>>(null);

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="interfaceType">The plugin interface type</param>
        /// <param name="name">The name of the plugin</param>
        /// <param name="description">The description of the plugin</param>
        /// <param name="version">The version of the plugin</param>
        /// <param name="author">The author of the plugin</param>
        /// <param name="identifier">The GUID identifier for the plugin</param>
        /// <param name="controllerType">The type of the controller for this plugin</param>
        public DistribPluginAttribute(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            string identifier,
            Type controllerType = null)
        {
            m_typInterfaceType = interfaceType;
            m_strName = name;
            m_strDescription = description;
            m_dVersion = version;
            m_strAuthor = author;
            m_strIdentifier = identifier;
            m_typControllerType.Value = controllerType;
        }

        /// <summary>
        /// Gets the plugin interface type
        /// </summary>
        public Type InterfaceType
        {
            get { return m_typInterfaceType; }
        }

        /// <summary>
        /// Gets the plugin name
        /// </summary>
        public string Name
        {
            get { return m_strName; }
        }

        /// <summary>
        /// Gets the plugin description
        /// </summary>
        public string Description
        {
            get { return m_strDescription; }
        }

        /// <summary>
        /// Gets the plugin version
        /// </summary>
        public double Version
        {
            get { return m_dVersion; }
        }

        /// <summary>
        /// Gets the plugin author
        /// </summary>
        public string Author
        {
            get { return m_strAuthor; }
        }

        /// <summary>
        /// Gets the plugin identifier
        /// </summary>
        public string Identifier
        {
            get { return m_strIdentifier; }
        }

        /// <summary>
        /// Gets the controller type, null if not set
        /// </summary>
        public Type ControllerType
        {
            get
            {
                lock (m_typControllerType)
                {
                    return (!m_typControllerType.IsWritten) ? null : m_typControllerType.Value;
                }
            }

            set
            {
                lock (m_typControllerType)
                {
                    if (!m_typControllerType.IsWritten)
                    {
                        m_typControllerType.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Controller type already specified");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the readonly list of the additional metadata supplied by the derived attribute, null if not set
        /// </summary>
        public IReadOnlyList<IDistribPluginAdditionalMetadataBundle> SuppliedAdditionalMetadata
        {
            get
            {
                lock (m_lstSuppliedAdditionalMetadata)
                {
                    return (!m_lstSuppliedAdditionalMetadata.IsWritten) ? null : m_lstSuppliedAdditionalMetadata.Value;
                }
            }

            protected set
            {
                lock (m_lstSuppliedAdditionalMetadata)
                {
                    if (!m_lstSuppliedAdditionalMetadata.IsWritten)
                    {
                        m_lstSuppliedAdditionalMetadata.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Supplied additional metadata already set");
                    }
                }
            }
        }
    }
}
