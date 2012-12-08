using Distrib.Utils;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Distrib.Plugins
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class PluginAdditionalMetadataAttribute : Attribute
    {
        private readonly IKernel _kernel;

        private readonly Type _metadataInterface = null;
        private readonly string _metadataIdentity = Guid.NewGuid().ToString();
        private readonly PluginMetadataBundleExistencePolicy _existencePolicy =
            PluginMetadataBundleExistencePolicy.NotImportant;

        private WriteOnce<IReadOnlyList<PropertyInfo>> _metadataProperties =
            new WriteOnce<IReadOnlyList<PropertyInfo>>(null);

        private PluginAdditionalMetadataAttribute() { }

        protected PluginAdditionalMetadataAttribute(IKernel kernel,
            Type metadataInterfaceType,
            string identity,
            PluginMetadataBundleExistencePolicy existencePolicy)
        {
            _kernel = kernel;
            _metadataInterface = metadataInterfaceType;
            _metadataIdentity = identity;
            _existencePolicy = existencePolicy;
        }

        private object _doMetadataReturn()
        {
            if (_metadataInterface == null) throw new InvalidOperationException("No metadata type has been provided");
            if (!_metadataInterface.IsInterface) throw new InvalidOperationException("Metadata type must be an interface");

            try
            {
                var metadataObject = _provideMetadata();

                // Make sure object is serializable
                if (metadataObject.GetType().GetCustomAttribute<SerializableAttribute>() == null)
                {
                    throw new InvalidOperationException("Metadata concrete class must be serializable");
                }

                // Make sure the metadata object actually casts to the interface type
                if (!_metadataInterface.IsAssignableFrom(metadataObject.GetType()))
                {
                    throw new InvalidOperationException("Metadata object not assignable from interface type");
                }

                return metadataObject;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata", ex);
            }
        }

        protected abstract object _provideMetadata();

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
                _metadataProperties.Value =
                    _metadataInterface.GetProperties()
                    .Where(p => p.GetGetMethod(false) != null && p.GetSetMethod(false) != null)
                    .ToList()
                    .AsReadOnly();

                // Make sure there are some
                if (_metadataProperties.Value == null ||
                    _metadataProperties.Value.Count == 0)
                {
                    throw new InvalidOperationException("The metadata type provided has no properties with public getters and setters");
                }

                // Run through and pull out the name and value
                foreach (var pi in _metadataProperties.Value)
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

        internal IPluginMetadataBundle ToMetadataBundle()
        {
            try
            {
                //return new ConcreteDistribPluginAdditionalMetadataBundle(m_typMetadataInterface,
                //    this.GetType(), _doMetadataReturn(), ProvideMetadataKVPs(), m_strMetadataIdentity, m_enumIdentityPolicy);

#warning Hacky way of getting readonly dictionary, providemetadatakvps needs changing
                return _kernel.Get<IPluginMetadataBundleFactory>()
                    .CreateBundle(
                        _metadataInterface,
                        this.GetType(),
                        _doMetadataReturn(),
                        new ReadOnlyDictionary<string, object>(ProvideMetadataKVPs()),
                        _metadataIdentity,
                        _existencePolicy);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get metadata bundle", ex);
            }
        }
    }
}
