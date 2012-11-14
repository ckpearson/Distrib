using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DistribPluginAttribute : Attribute
    {
        private readonly string m_strName = "";
        private readonly string m_strDescription = "";
        private readonly double m_dVersion = 0.0;
        private readonly string m_strAuthor = "";

        private readonly Type m_typInterfaceType = null;

        private readonly WriteOnce<Type> m_typControllerType = new WriteOnce<Type>(null);

        public DistribPluginAttribute(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            Type controllerType = null)
        {
            m_typInterfaceType = interfaceType;
            m_strName = name;
            m_strDescription = description;
            m_dVersion = version;
            m_strAuthor = author;
            m_typControllerType.Value = controllerType;
        }

        public Type InterfaceType
        {
            get { return m_typInterfaceType; }
        }

        public string Name
        {
            get { return m_strName; }
        }

        public string Description
        {
            get { return m_strDescription; }
        }

        public double Version
        {
            get { return m_dVersion; }
        }

        public string Author
        {
            get { return m_strAuthor; }
        }

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
    }
}
