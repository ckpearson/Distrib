using Distrib.Plugins.Description;
using Distrib.Plugins.Discovery;
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
    internal sealed class DistribDefaultPluginController : MarshalByRefObject, 
        IDistribPluginController,
        IDistribPluginControllerInterface
    {
        private RemoteAppDomainBridge m_remBridge = null;
        private IDistribPlugin m_objInstance = null;
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


        void IDistribPluginController.InitController()
        {
            // Controller initialisation
        }

        void IDistribPluginController.UnitController()
        {
            // Controller unitialisation (not to unitialise instance here)
        }

        void IDistribPluginController.StoreAppDomainBridge(RemoteAppDomainBridge bridge)
        {
            m_remBridge = bridge;
        }

        object IDistribPluginController.CreatePluginInstance(DistribPluginDetails pluginDetails, string parentAssemblyPath)
        {
            _updatePluginDetails(pluginDetails);

            m_objInstance = (IDistribPlugin)m_remBridge.CreateInstance(pluginDetails.PluginTypeName,
                parentAssemblyPath);

            return m_objInstance;
        }

        void IDistribPluginController.InitialiseInstance()
        {
            m_objInstance.InitPlugin(this);
        }

        void IDistribPluginController.UnitialiseInstance()
        {
            m_objInstance.UninitPlugin(this);
        }

        Discovery.Metadata.DistribPluginMetadata IDistribPluginControllerInterface.PluginMetadata
        {
            get
            {
                lock (m_lock)
                {
                    return m_pluginDetails.Metadata;
                }
            }
        }

        private WeakReference<IReadOnlyList<IDistribPluginAdditionalMetadataBundle>> m_rOnlyBundles
            = null;
        IReadOnlyList<IDistribPluginAdditionalMetadataBundle> IDistribPluginControllerInterface.AdditionalMetadata
        {
            get
            {
                lock (m_lock)
                {
                    IReadOnlyList<IDistribPluginAdditionalMetadataBundle> lst = null;

                    if (m_rOnlyBundles == null)
                    {
                        lst = m_pluginDetails.AdditionalMetadataBundles.AsReadOnly();
                        m_rOnlyBundles = new WeakReference<IReadOnlyList<IDistribPluginAdditionalMetadataBundle>>(lst);
                    }
                    else
                    {
                        if (!m_rOnlyBundles.TryGetTarget(out lst))
                        {
                            lst = m_pluginDetails.AdditionalMetadataBundles.AsReadOnly();
                            m_rOnlyBundles.SetTarget(lst);
                        }
                    }

                    return lst;
                }
            }
        }




       
    }
}
