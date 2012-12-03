using Distrib.Plugins_old.Controllers;
using Distrib.Plugins_old.Description;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old
{
    public sealed class DistribPluginInstance
    {
        private readonly WriteOnce<bool> m_bInitialisedOnce = new WriteOnce<bool>(false);

        private readonly PluginDetails m_pluginDetails = null;
        private readonly DistribPluginAssembly m_parentAssembly = null;

        private AppDomain m_adAppDomain = null;
        private RemoteAppDomainBridge m_appDomainBridge = null;

        private object m_lock = new object();
        private bool m_bIsInitialised = false;

        private WriteOnce<object> m_instance = new WriteOnce<object>(null);

        private WriteOnce<IDistribPluginController> m_pluginController = new WriteOnce<IDistribPluginController>(null);

        private readonly string m_strGuid = null;
        private readonly WriteOnce<DateTime> m_dtInstanceCreationStamp = new WriteOnce<DateTime>();

        internal DistribPluginInstance(PluginDetails pluginDetails, DistribPluginAssembly parentAssembly)
        {
            m_pluginDetails = pluginDetails;
            m_parentAssembly = parentAssembly;
            m_strGuid = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the unique instance ID
        /// </summary>
        public string InstanceID
        {
            get
            {
                return m_strGuid;
            }
        }

        /// <summary>
        /// Gets the date-time when the actual instance was created, returns <see cref="DateTime.MinValue"/> if the instance
        /// hasn't been created yet.
        /// </summary>
        public DateTime InstanceCreationStamp
        {
            get
            {
                lock (m_lock)
                {
                    if (!IsInitialised() || !m_instance.IsWritten)
                    {
                        return DateTime.MinValue;
                    }
                    else
                    {
                        return m_dtInstanceCreationStamp.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the actual instance object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : class
        {
            try
            {
                lock (m_lock)
                {
                    if (!typeof(T).IsInterface)
                    {
                        throw new InvalidOperationException("T must be an interface");
                    }

                    if (!typeof(T).Equals(m_pluginDetails.Metadata.InterfaceType))
                    {
                        throw new InvalidOperationException("T must be of same interface type for plugin metadata");
                    }

                    if (!IsInitialised())
                    {
                        Initialise();
                    }

                    // Now create the controller

                    if ((m_pluginDetails.Metadata.ControllerType.Assembly.Location !=
                        Assembly.GetExecutingAssembly().Location) &&
                        (m_pluginDetails.Metadata.ControllerType.Assembly.Location !=
                            m_parentAssembly.AssemblyFilePath))
                    {
                        // The controller type is in a different assembly than the Distrib assembly
                        // and the assembly the plugin is in, the assembly needs loading into the appdomain

                        m_appDomainBridge.LoadAssembly(m_pluginDetails.Metadata.ControllerType.Assembly.Location);
                    }

                    // Now create the actual controller instance

                    m_pluginController.Value = (IDistribPluginController)m_appDomainBridge.CreateInstance(
                        m_pluginDetails.Metadata.ControllerType.FullName,
                        m_pluginDetails.Metadata.ControllerType.Assembly.Location);

                    // Initialise the controller
                    m_pluginController.Value.InitController();

                    // Send over the app domain bridge so the controller can create instance
                    m_pluginController.Value.StoreAppDomainBridge(m_appDomainBridge);

                    if (!m_instance.IsWritten)
                    {
                        m_instance.Value = m_pluginController.Value.CreatePluginInstance(m_pluginDetails,
                            m_parentAssembly.AssemblyFilePath);
                    }

                    // Now get the controller to initialise the plugin

                    m_pluginController.Value.InitialiseInstance();

                    return (T)m_instance.Value;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get instance", ex);
            }
        }

        #region AppDomain Initialisation Logic

        /// <summary>
        /// Initialises the plugin instance (creates AppDomain etc)
        /// </summary>
        internal void Initialise()
        {
            try
            {
                lock (m_lock)
                {
                    // Only support initialisation once
                    if (m_bInitialisedOnce.IsWritten)
                    {
                        if (m_bInitialisedOnce.Value)
                        {
                            throw new InvalidOperationException("Cannot initialise; been initialised once before, re-create instance");
                        }
                    }

                    if (IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot initialise; already initialised");
                    }

                    // Set up app domain
                    m_adAppDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + m_parentAssembly.AssemblyFilePath + "_" +
                        m_pluginDetails.PluginTypeName);

                    // Create remote bridge
                    m_appDomainBridge = RemoteAppDomainBridge.FromAppDomain(m_adAppDomain);

                    // Load the distrib assembly into the AppDomain
                    m_appDomainBridge.LoadAssembly(Assembly.GetExecutingAssembly().Location);

                    // Load the plugin assembly into the AppDomain
                    m_appDomainBridge.LoadAssembly(m_parentAssembly.AssemblyFilePath);

                    m_instance = new WriteOnce<object>(null);

                    m_bIsInitialised = true;
                    m_bInitialisedOnce.Value = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise", ex);
            }
        }

        /// <summary>
        /// Determines whether the plugin instance has been initialised
        /// </summary>
        /// <returns><c>True</c> if so, <c>False</c> otherwise</returns>
        internal bool IsInitialised()
        {
            try
            {
                lock (m_lock)
                {
                    return m_bIsInitialised;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if initialised", ex);
            }
        }

        /// <summary>
        /// Unitialises the plugin instance
        /// </summary>
        internal void Unitialise()
        {
            try
            {
                lock (m_lock)
                {
                    if (!IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot unitialise; not initialised");
                    }

                    // Get the controller to unitialise the instance
                    m_pluginController.Value.UnitialiseInstance();

                    // Unitialise the controller
                    m_pluginController.Value.UnitController();

                    // Destroy bridge
                    m_appDomainBridge = null;

                    // Unload AppDomain
                    AppDomain.Unload(m_adAppDomain);

                    // Cleanup
                    m_adAppDomain = null;
                    m_bIsInitialised = false;
                    m_instance = null;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to unitialise", ex);
            }
        }

        #endregion
    }
}
