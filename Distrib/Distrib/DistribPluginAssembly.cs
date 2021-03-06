﻿using Distrib.Plugins.Controllers;
using Distrib.Plugins.Description;
using Distrib.Plugins.Discovery;
using Distrib.Processes;
using Distrib.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public sealed class DistribPluginAssembly
    {
        private readonly string m_strAssemblyPath = "";

        private AppDomain m_adAssemblyAppDomain = null;
        private DistribPluginAssemblyManager m_asmManager = null;

        private object m_objLock = new object();

        private bool m_bIsInitialised = false;

        private IReadOnlyList<PluginDetails> m_lstPluginDetails = null;

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly</param>
        private DistribPluginAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("AssemblyPath must not be null or empty");

            m_strAssemblyPath = assemblyPath;
        }

        /// <summary>
        /// Performs initialisation and loads the assembly
        /// </summary>
        public DistribPluginAssemblyInitialisationResult Initialise()
        {
            DistribPluginAssemblyInitialisationResult result = new DistribPluginAssemblyInitialisationResult();

            try
            {
                lock (m_objLock)
                {
                    if (IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot initialise plugin assembly; it is already initialised");
                    }

                    // Set up the AppDomain
                    m_adAssemblyAppDomain = AppDomain.CreateDomain(Guid.NewGuid() + "_" + m_strAssemblyPath);

                    try
                    {
                        // Create the assembly manager for the plugin assembly isolated in the AppDomain
                        m_asmManager = (DistribPluginAssemblyManager)m_adAssemblyAppDomain
                                        .CreateInstanceAndUnwrap(
                                        typeof(DistribPluginAssemblyManager).Assembly.FullName,
                                        typeof(DistribPluginAssemblyManager).FullName,
                                        true,
                                        System.Reflection.BindingFlags.CreateInstance,
                                        null,
                                        new object[] { m_strAssemblyPath },
                                        null,
                                        null);

                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to create manager in AppDomain", ex);
                    }

                    if (m_asmManager == null)
                    {
                        throw new ApplicationException("Manager in AppDomain was null after creation");
                    }

                    try
                    {
                        // Load the assembly into the domain
                        m_asmManager.LoadAssembly();
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Manager failed to load assembly into AppDomain", ex);
                    }

                    // Retrieve the details for all plugins present in the assembly
                    // At this point these are the details that should be utilised by the developer and the system
                    // for *actual* work as they are serialised through the AppDomain and as a result
                    // any modifications here are not represented in the manager (usability etc)
                    // When pushing down to the manager to perform low-level work, the type-name is used as the key
                    // and the plugin details data will simply be used from the version given to the manager.
                    m_lstPluginDetails = m_asmManager.GetPluginDetails();

                    if (m_lstPluginDetails == null || m_lstPluginDetails.Count == 0)
                    {
                        throw new InvalidOperationException("The plugin assembly contains no Distrib Plugins");
                    }

                    // Go through all the plugins in the assembly
                    foreach (var pluginType in m_lstPluginDetails)
                    {
                        // Do any bootstrapping that's required to fill in defaults etc
                        // This has to be done before checking the usability as it could
                        // affect things.
                        var bootstrapResult = _PerformPluginBootstrapping(pluginType);

                        if (!bootstrapResult.Success)
                        {
                            pluginType.MarkAsUnusable(PluginExclusionReason.PluginBootstrapFailure,
                                bootstrapResult.Result);

                            result.AddPlugin(pluginType);

                            continue;
                        }

                        // Check over the usability of the plugin and mark it accordingly
                        var usabilityCheckResult = _CheckUsabilityOfPlugin(pluginType);

                        if (!usabilityCheckResult.Success)
                        {
                            pluginType.MarkAsUnusable(usabilityCheckResult.Result, usabilityCheckResult.ResultTwo);

                            result.AddPlugin(pluginType);

                            continue;
                        }

                        // Make sure that the additional metadata meets the existence / instancing requirements specified
                        var metadataConstraintsResult = _CheckAdditionalMetadataExistenceRequirements(pluginType);

                        if (!metadataConstraintsResult.Success)
                        {
                            pluginType.MarkAsUnusable(PluginExclusionReason.PluginAdditionalMetadataConstraintsNotMet,
                                metadataConstraintsResult.Result);

                            result.AddPlugin(pluginType);

                            continue;
                        }

                        pluginType.MarkAsUsable();
                        result.AddPlugin(pluginType);
                    }

                    m_bIsInitialised = true;
                }

                // Lock the result to ensure no modifications
                result.LockResult();
                return result;
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Something has gone wrong when trying to load the types in the plugin
                // Mostly owing to missing references / plugins not adhering to interfaces

                var loaderExceptions = ex.LoaderExceptions;

                var sb = new StringBuilder();
                sb.AppendLine("Failed to initialise plugin assembly, type loading errors:");

                foreach (var te in loaderExceptions)
                {
                    sb.AppendLine(te.Message);
                }

                throw new ApplicationException(sb.ToString(), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialise plugin assembly", ex);
            }
        }

        private void _ProcessUsabilityResult(PluginDetails pluginType, 
            Res<PluginExclusionReason, object> usabilityCheckResult)
        {
            if (usabilityCheckResult.Success)
            {
                pluginType.MarkAsUsable();
            }
            else
            {
                pluginType.MarkAsUnusable(usabilityCheckResult.Result);
            }
        }

        /// <summary>
        /// Checks that a given plugin's additional metadata existence policies are met
        /// </summary>
        /// <param name="pluginType">The plugin details</param>
        /// <returns>The result of the check</returns>
        private Res<List<Tuple<IPluginAdditionalMetadataBundle, AdditionalMetadataExistenceCheckResult, string>>> 
            _CheckAdditionalMetadataExistenceRequirements(PluginDetails pluginType)
        {
            var resultList = new List<Tuple<IPluginAdditionalMetadataBundle, AdditionalMetadataExistenceCheckResult, string>>();
            bool success = true;

            // Simple action to add a bunch of bundles with the result and reason
            Action<IEnumerable<IPluginAdditionalMetadataBundle>,
                AdditionalMetadataExistenceCheckResult, string> addRange = (bundles, res, msg) =>
                    {
                        lock(resultList)
                        {
                            resultList.AddRange(bundles.Select(b => 
                                new Tuple<IPluginAdditionalMetadataBundle, AdditionalMetadataExistenceCheckResult, string>(
                                     b, res, msg)));
                        }
                    };

            // Check there are some metadata bundles
            if (pluginType.AdditionalMetadataBundles != null && pluginType.AdditionalMetadataBundles.Count > 0)
            {
                // Group by the identities
                var grps = pluginType.AdditionalMetadataBundles.GroupBy(b => b.MetadataInstanceIdentity);

                foreach (var group in grps)
                {
                    // The whole identity group have to agree on their existence policy
                    if (group.GroupBy(b => b.MetadataInstanceExistencePolicy).Count() != 1)
                    {
                        addRange(group, AdditionalMetadataExistenceCheckResult.ExistencePolicyMismatch, "Existence policy not agreed upon across identity");
                        success &= false;
                    }
                    else
                    {
                        // Perform checks on policy
                        switch (group.First().MetadataInstanceExistencePolicy)
                        {
                            // The group should only have a single instance present
                            case Discovery.Metadata.AdditionalPluginMetadataIdentityExistencePolicy.SingleInstance:

                                if (group.Count() == 1)
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.Success, "Succeeded as policy called for single instance");
                                    success &= true;
                                }
                                else
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.FailedExistencePolicy, "Failed as policy called for single instance");
                                    success &= false;
                                }
                                break;

                            // The group should have at least a single instance
                            case Discovery.Metadata.AdditionalPluginMetadataIdentityExistencePolicy.AtLeastOne:

                                if (group.Count() >= 1)
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.Success,
                                        "Succeeded as policy called for at least one instance and '{0}' {1}"
                                        .fmt(group.Count(), group.Count() == 1 ? "was" : "were"));
                                    success &= true;
                                }
                                else
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.FailedExistencePolicy,
                                        "Failed as policy called for at least one instance");
                                    success &= false;
                                }

                                break;

                            // The group must have multiple instances
                            case Discovery.Metadata.AdditionalPluginMetadataIdentityExistencePolicy.MultipleInstances:

                                if (group.Count() > 1)
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.Success, "Succeeded as policy called for multiple instances");
                                    success &= true;
                                }
                                else
                                {
                                    addRange(group, AdditionalMetadataExistenceCheckResult.FailedExistencePolicy, "Failed as policy called for multiple instances");
                                    success &= false;
                                }
                                break;

                            // It's simply not important, so just throw all of them in as OK
                            default:
                            case Discovery.Metadata.AdditionalPluginMetadataIdentityExistencePolicy.NotImportant:

                                addRange(group, AdditionalMetadataExistenceCheckResult.Success, "Succeeded as policy not important");
                                success &= success = true;
                                break;
                        }
                    }
                }
            }
            return new Res<List<Tuple<IPluginAdditionalMetadataBundle, AdditionalMetadataExistenceCheckResult, string>>>((success &= true),
                resultList);
        }

        /// <summary>
        /// Performs the bootstrapping for a given plugin's details so any defaults can be set if needed
        /// </summary>
        /// <param name="pluginType">The plugin details</param>
        /// <returns>The result</returns>
        private Res<DistribPluginBootstrapResult> _PerformPluginBootstrapping(PluginDetails pluginType)
        {
            DistribPluginBootstrapResult res = DistribPluginBootstrapResult.Success;

            if (pluginType == null) throw new ArgumentNullException("Plugin details must be supplied");

            try
            {
                // If the plugin hasn't specified a controller then that needs to be the default controller
                if (pluginType.Metadata.ControllerType == null)
                {
                    pluginType.Metadata.ControllerType = typeof(DistribDefaultPluginController);
                }

                return new Res<DistribPluginBootstrapResult>(res == DistribPluginBootstrapResult.Success, res);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to perform bootstrapping for plugin", ex);
            }
        }

        private readonly Dictionary<PluginDetails, List<DistribPluginInstance>>
            m_dictInstances = new Dictionary<PluginDetails, List<DistribPluginInstance>>();

        public DistribPluginInstance CreatePluginInstance(PluginDetails details)
        {
            if (details == null) throw new ArgumentNullException("Plugin details must be supplied");

            DistribPluginInstance instance = null;

            try
            {
                if (!details.IsUsable)
                {
                    throw new InvalidOperationException("Plugin is marked as excluded because: {0}".fmt(
                        details.ExclusionReason.ToString()));
                }

                lock (m_objLock)
                {
                    lock (m_dictInstances)
                    {
                        List<DistribPluginInstance> lstInstances = null;
                        if (m_dictInstances.TryGetValue(details, out lstInstances) == false)
                        {
                            lstInstances = new List<DistribPluginInstance>();
                        }

                        instance = new DistribPluginInstance(details, this);
                        lstInstances.Add(instance);

                        m_dictInstances[details] = lstInstances;
                    }
                }

                return instance;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create plugin instance", ex);
            }
        }

        private Res<PluginExclusionReason, object> _CheckUsabilityOfPlugin(PluginDetails pluginType)
        {
            var res = PluginExclusionReason.Unknown;
            object resultAddit = null;
            bool success = true;

            if (pluginType == null) throw new ArgumentNullException("Plugin Details must be supplied");

            try
            {
                var _result = CChain<Tuple<PluginExclusionReason, object>>
                    // Check it's marshalable
                    .If(() => !m_asmManager.PluginTypeIsMarshalable(pluginType),
                        new Tuple<PluginExclusionReason, object>(PluginExclusionReason.TypeNotMarshalable, null))
                    // Check it implements the IDistribPlugin interface
                    .ThenIf(() => !m_asmManager.PluginTypeImplementsDistribPluginInterface(pluginType),
                        new Tuple<PluginExclusionReason,object>(PluginExclusionReason.DistribPluginInterfaceNotImplemented, null))
                    // Check it adheres to the plugin interface as stated
                    .ThenIf(() => !m_asmManager.PluginTypeAdheresToStatedInterface(pluginType),
                        new Tuple<PluginExclusionReason, object>(PluginExclusionReason.NonAdherenceToInterface, null))
                    // Check that the specified plugin controller is valid
                    .ThenIf(() => !DistribPluginControllerSystem.ValidateControllerType(pluginType.Metadata.ControllerType).Success,
                        new Tuple<PluginExclusionReason, object>(PluginExclusionReason.PluginControllerInvalid,
                            DistribPluginControllerSystem.ValidateControllerType(pluginType.Metadata.ControllerType).ResultTwo))
                    .Result;

                if (_result == null)
                {
                    // No result above means that the checks all came out fine
                    success = true;
                    res = PluginExclusionReason.Unknown;
                    resultAddit = null;
                }
                else
                {
                    // Something did go wrong
                    success = false;
                    res = _result.Item1;
                    resultAddit = _result.Item2;
                }

                return new Res<PluginExclusionReason, object>((success &= true), res, resultAddit);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check usability of plugin", ex);
            }
        }

        public void Uninitialise()
        {
            try
            {
                lock (m_objLock)
                {
                    if (!IsInitialised())
                    {
                        throw new InvalidOperationException("Cannot unitialise plugin assembly; it isn't initialised");
                    }

                    // If there are any plugin instances they need tearing down
                    if (m_dictInstances != null && m_dictInstances.Count > 0)
                    {
                        foreach (var instance in m_dictInstances.Values.SelectMany(l => l).Where(l => l.IsInitialised()))
                        {
                            instance.Unitialise();
                        }
                    }

                    m_asmManager = null;

                    try
                    {
                        AppDomain.Unload(m_adAssemblyAppDomain);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Failed to unload plugin assembly AppDomain", ex);
                    }

                    m_adAssemblyAppDomain = null;

                    m_bIsInitialised = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to unitialise plugin assembly", ex);
            }
        }

        public bool IsInitialised()
        {
            try
            {
                lock (m_objLock)
                {
                    return m_bIsInitialised;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to determine if assembly is already initialised", ex);
            }
        }

        public string AssemblyFilePath
        {
            get
            {
                return m_strAssemblyPath;
            }
        }

        public static DistribPluginAssembly CreateForAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("AssemblyPath must not be null or empty");

            try
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException("Plugin assembly file not found", assemblyPath);
                }

                return new DistribPluginAssembly(assemblyPath);
            }
            catch (Exception ex)
            {
                throw new FailedToLoadDistribPluginAssemblyException(assemblyPath,
                    "Failed to load plugin assembly", ex);
            }
        }
    }

    [Serializable]
    public class FailedToLoadDistribPluginAssemblyException : Exception
    {
        private readonly string m_strAssemblyPath = "";

        public FailedToLoadDistribPluginAssemblyException() { }
        public FailedToLoadDistribPluginAssemblyException(string assemblyPath, string message)
            : base(message)
        {
            m_strAssemblyPath = assemblyPath;
        }
        public FailedToLoadDistribPluginAssemblyException(string assemblyPath, string message, Exception inner)
            : base(message, inner)
        {
            m_strAssemblyPath = assemblyPath;
        }
        protected FailedToLoadDistribPluginAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
