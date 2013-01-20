/*
	This software known as 'Distrib' at time of creation is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	These following points are generalisations of the terms of the license and as such you MUST read the license itself
	in order to correctly know your rights and responsibilities.

	Primarily the license states:
		> You ARE ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is
			1. Free to use (though you may charge for distribution)
			2. Free to study and modify (though you may charge for distribution of the source code)
			3. Available under the same license as this software

		> You ARE NOT ALLOWED to use this software be it in binary form or in source form (whole or part) in any software that is:
			1. Commercial (this covers any software for which there is a fee to use and procure)
			2. Software that is closed-source (even if the source is available for a fee)
			3. Not available under the same license as this software

	If this software has been made available to you under any license other than the original license by any party other than the
	original copyright holder (Clint Pearson) then they have acted under breach of their original agreement.

	If this software has been made available to you for a fee for distribution by any party other than the original copyright holder (Clint Pearson)
	then they have acted under breach of their original agreement unless this software is a derivative created by that party.

	If you have received this software from the original copyright holder (Clint Pearson) and it has been made available to you under
	the terms of the original license and you wish to obtain a different license to cover your use of the software, then you may contact
	the copyright holder to negotiate a new license.
*/
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
    public sealed class RemoteAppDomainBridge : CrossAppDomainObject
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
