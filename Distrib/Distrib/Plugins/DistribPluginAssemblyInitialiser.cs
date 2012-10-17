using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Distrib.Plugins.Discovery;

namespace Distrib.Plugins
{
    [Serializable()]
    internal sealed class DistribPluginAssemblyManager : MarshalByRefObject
    {
        private readonly string m_strPluginAssemblyPath = "";
        private Assembly m_asmPluginAssembly = null;

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

        public List<DistribPluginType> GetPluginTypes()
        {
            var types = m_asmPluginAssembly.GetTypes()
                .Where(t => t.GetCustomAttribute<DistribPluginAttribute>() != null).ToArray();

            var l = new List<DistribPluginType>();
            foreach (var t in types.Select(t => new { type = t, attr = t.GetCustomAttribute<DistribPluginAttribute>() }))
            {
                l.Add(new DistribPluginType(t.type.FullName, t.attr.Name, t.attr.InterfaceType));
            }

            return l;
        }
    }

    [Serializable()]
    public sealed class DistribPluginDetails
    {
       
    }

    [Serializable()]
    internal sealed class DistribPluginType
    {
        private string m_strTypeName = "";
        private string m_strPluginName = "";
        private Type m_typPluginType = null;

        public DistribPluginType(string TypeName, string PluginName, Type pluginInterfaceType)
        {
            m_strTypeName = TypeName;
            m_strPluginName = PluginName;
            m_typPluginType = pluginInterfaceType;
        }

        public string PluginTypeName { get { return m_strTypeName; } }
        public Type PluginInterfaceType { get { return m_typPluginType; } }


    }
}
