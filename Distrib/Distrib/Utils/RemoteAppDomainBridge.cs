using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Utils
{
    /// <summary>
    /// Simple bridge object for use within an AppDomain
    /// </summary>
    public sealed class RemoteAppDomainBridge : MarshalByRefObject
    {
        private readonly Dictionary<string, Assembly> m_dictAssemblies =
            new Dictionary<string, Assembly>();

        private readonly object m_lock = new object();

        public void LoadAssembly(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("file path must be supplied");
            if (!File.Exists(filePath)) throw new FileNotFoundException("File path not found");

            try
            {
                lock (m_lock)
                {
                    if (m_dictAssemblies.ContainsKey(filePath))
                    {
                        throw new InvalidOperationException("Already hold an entry for assembly with this file path");
                    }
                    else
                    {
                        var asm = Assembly.LoadFrom(filePath);

                        if (asm != null)
                        {
                            m_dictAssemblies.Add(filePath, asm);
                        }
                        else
                        {
                            throw new ApplicationException("Assembly came back null!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load assembly", ex);
            }
        }

        public object CreateInstance(string typeName, string assemblyPath)
        {
            if (string.IsNullOrEmpty(typeName)) throw new ArgumentNullException("type name must be supplied");
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("assembly path must be supplied");

            try
            {
                lock (m_lock)
                {
                    if (!m_dictAssemblies.ContainsKey(assemblyPath))
                    {
                        throw new InvalidOperationException("No assembly with that path has been loaded");
                    }

                    return Activator.CreateInstance(m_dictAssemblies[assemblyPath].GetType(typeName));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create instance", ex);
            }
        }

        public static RemoteAppDomainBridge FromAppDomain(AppDomain domain)
        {
            if (domain == null) throw new ArgumentNullException("App domain must be supplied");

            try
            {
                return (RemoteAppDomainBridge)domain.CreateInstanceAndUnwrap(
                    typeof(RemoteAppDomainBridge).Assembly.FullName,
                    typeof(RemoteAppDomainBridge).FullName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create app domain bridge in domain", ex);
            }
        }
    }
}
