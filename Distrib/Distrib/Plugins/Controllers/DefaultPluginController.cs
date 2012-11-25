using Distrib.Plugins.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Controllers
{
    /// <summary>
    /// The default system-provided plugin controller
    /// </summary>
    internal sealed class DistribDefaultPluginController : MarshalByRefObject, IDistribPluginController
    {
        private RemoteAppDomainBridge m_remBridge = null;
        private object m_objInstance = null;
        private string m_strParentAssemblyPath = "";
        private DistribPluginDetails m_pluginDetails = null;

        private object m_lock = new object();

        private void _updatePluginDetails(DistribPluginDetails details)
        {
            lock (m_lock)
            {
                if (m_pluginDetails == null)
                {
                    m_pluginDetails = details;
                }
                else
                {
                    if (m_pluginDetails.Metadata.Identifier != details.Metadata.Identifier)
                    {
                        throw new InvalidOperationException("The identifier differs for the new plugin");
                    }
                    else
                    {
                        m_pluginDetails = details;
                    }
                }
            }
        }

        public DistribDefaultPluginController() { }


        void IDistribPluginController.StoreAppDomainBridge(RemoteAppDomainBridge bridge)
        {
            m_remBridge = bridge;
        }

        object IDistribPluginController.CreatePluginInstance(DistribPluginDetails pluginDetails, string parentAssemblyPath)
        {
            _updatePluginDetails(pluginDetails);

            m_objInstance = m_remBridge.CreateInstance(pluginDetails.PluginTypeName,
                parentAssemblyPath);

            return m_objInstance;
        }
    }
}
