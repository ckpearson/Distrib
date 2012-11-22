using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery.Metadata
{
    public enum AdditionalMetadataIdentityExistencePolicy
    {
        NotImportant = 0,
        SingleInstance,
        MultipleInstances,
    }

    /// <summary>
    /// Provides a means by which plugins can carry additional subsystem specific metadata along with it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DistribPluginAdditionalMetadataAttribute : Attribute
    {
        private readonly Type m_typMetadataInterface = null;
        private readonly string m_strMetadataIdentity = Guid.NewGuid().ToString();
        private readonly AdditionalMetadataIdentityExistencePolicy m_enumIdentityPolicy = AdditionalMetadataIdentityExistencePolicy.NotImportant;

        private WriteOnce<IReadOnlyList<PropertyInfo>> m_readOnlyListMetadataProperties =
            new WriteOnce<IReadOnlyList<PropertyInfo>>(null);

        private DistribPluginAdditionalMetadataAttribute() { }
        
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="metadataInterfaceType">The interface type that the metadata takes the form of</param>
        /// <param name="identity">The identifier used to represent this type of additional metadata</param>
        protected DistribPluginAdditionalMetadataAttribute(Type metadataInterfaceType, string identity,
            AdditionalMetadataIdentityExistencePolicy identityPolicy)
        {
            m_typMetadataInterface = metadataInterfaceType;
            m_strMetadataIdentity = identity;
            m_enumIdentityPolicy = identityPolicy;
        }

        /// <summary>
        /// Handles the returning of the metadata object from the derived attribute
        /// </summary>
        /// <returns>The metadata object provided by the derived attribute</returns>
        private object _doMetadataReturn()
        {
            if (m_typMetadataInterface == null) throw new InvalidOperationException("No metadata type has been provided");
            if (!m_typMetadataInterface.IsInterface) throw new InvalidOperationException("Metadata type must be an interface");

            try
            {
                var metadataObject = _provideMetadata();

                // Make sure the metadata object is serializable
                if (metadataObject.GetType().GetCustomAttribute<SerializableAttribute>() == null)
                {
                    throw new InvalidOperationException("Metadata concrete class must be serializable");
                }

                // Make sure that the metadata object can actually cast across to the interface type
                if (!m_typMetadataInterface.IsAssignableFrom(metadataObject.GetType()))
                {
                    throw new InvalidOperationException("Returned metadata object is not assignable from the metadata interface type");
                }

                return metadataObject;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata", ex);
            }
        }

        /// <summary>
        /// When overriden in a derived class, returns the metadata object holding the additional metadata
        /// </summary>
        /// <returns>The metadata object</returns>
        protected abstract object _provideMetadata();

        /// <summary>
        /// Gets the pure object holding the metadata
        /// </summary>
        /// <returns>The metadata object</returns>
        internal object ProvideMetadataInstance()
        {
            return _doMetadataReturn();
        }

        /// <summary>
        /// Gets the metadata object cast to the specified form
        /// </summary>
        /// <typeparam name="T">The metadata interface type</typeparam>
        /// <returns>The metadata interface instance</returns>
        internal T ProvideMetadataInstance<T>()
        {
            return (T)_doMetadataReturn();
        }

        /// <summary>
        /// Gets the key-value pairs of the metadata
        /// </summary>
        /// <returns>A dictionary containing the metadata key-value pairs</returns>
        internal Dictionary<string, object> ProvideMetadataKVPs()
        {
            var dict = new Dictionary<string, object>();

            try
            {
                var metadataObject = _doMetadataReturn();
                
                // Grab all the properties with public getters and setters
                m_readOnlyListMetadataProperties.Value =
                    m_typMetadataInterface.GetProperties()
                    .Where(p => p.GetGetMethod(false) != null && p.GetSetMethod(false) != null)
                    .ToList()
                    .AsReadOnly();

                // Make sure there are some
                if (m_readOnlyListMetadataProperties.Value == null ||
                    m_readOnlyListMetadataProperties.Value.Count == 0)
                {
                    throw new InvalidOperationException("The metadata type provided has no properties with public getters and setters");
                }

                // Run through and pull out the name and value
                foreach (var pi in m_readOnlyListMetadataProperties.Value)
                {
                    dict.Add(pi.Name, pi.GetValue(metadataObject));
                }

                return dict;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata key-values", ex);
            }
        }

        /// <summary>
        /// Generates a metadata bundle for the metadata presented by this attribute
        /// </summary>
        /// <returns>The <see cref="IDistribPluginAdditionalMetadataBundle"/> holding the metadata details</returns>
        internal IDistribPluginAdditionalMetadataBundle ToMetadataBundle()
        {
            try
            {

                return new ConcreteDistribPluginAdditionalMetadataBundle(m_typMetadataInterface,
                    this.GetType(), _doMetadataReturn(), ProvideMetadataKVPs(), m_strMetadataIdentity, m_enumIdentityPolicy);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata bundle", ex);
            }
        }
    }
}
