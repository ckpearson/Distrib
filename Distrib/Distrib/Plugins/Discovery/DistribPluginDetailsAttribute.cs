using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DistribPluginDetailsAttribute : Attribute
    {
        private readonly string m_strName = "";
        private readonly string m_strDescription = "";
        private readonly double m_dVersion = 0.0;
        private readonly string m_strAuthor = "";
        private readonly string m_strCopyright = "";

        private readonly Type m_typPluginInterface = null;

        public DistribPluginDetailsAttribute(
            Type pluginInterfaceType,
            string name,
            string description,
            double version,
            string author,
            string copyright = "")
        {
            m_typPluginInterface = pluginInterfaceType;
            m_strName = name;
            m_strDescription = description;
            m_dVersion = version;
            m_strAuthor = author;
            m_strCopyright = copyright;
        }
    }
}
