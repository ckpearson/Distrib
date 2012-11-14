﻿using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    /// <summary>
    /// Provides a means by which plugins can carry additional subsystem specific metadata along with it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DistribPluginAdditionalMetadataAttribute : Attribute
    {
        private readonly Type m_typMetadataInterface = null;
        private WriteOnce<IReadOnlyList<PropertyInfo>> m_readOnlyListMetadataProperties =
            new WriteOnce<IReadOnlyList<PropertyInfo>>(null);

        private DistribPluginAdditionalMetadataAttribute() { }
        
        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="metadataInterfaceType">The interface type that the metadata takes the form of</param>
        protected DistribPluginAdditionalMetadataAttribute(Type metadataInterfaceType)
        {
            m_typMetadataInterface = metadataInterfaceType;
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
        internal object ProvideMetadataVessel()
        {
            return _doMetadataReturn();
        }

        /// <summary>
        /// Gets the metadata object cast to the specified form
        /// </summary>
        /// <typeparam name="T">The metadata interface type</typeparam>
        /// <returns>The metadata interface instance</returns>
        internal T ProvideMetadataVessel<T>()
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

        internal IDistribPluginAdditionalMetadataBundle ToMetadataBundle()
        {
            return new Concrete_DistribPluginAdditionalMetadataBundle(
                m_typMetadataInterface, _doMetadataReturn(), this.GetType(), ProvideMetadataKVPs());
        }
    }

    public interface IDistribPluginAdditionalMetadataBundle
    {
        Type AdditionalMetadataAttributeType { get; }
        T GetMetadataObject<T>();
        object GetMetadataObject();
        Dictionary<string, object> MetadataKVPs { get; }
    }

    [Serializable()]
    internal sealed class Concrete_DistribPluginAdditionalMetadataBundle
        : IDistribPluginAdditionalMetadataBundle
    {
        private readonly Type m_type = null;
        private readonly object m_metadataObject = null;
        private readonly Type m_attrType = null;
        private readonly Dictionary<string, object> m_dictKVP = new Dictionary<string, object>();

        internal Concrete_DistribPluginAdditionalMetadataBundle(Type type, 
            object metadataObject, Type attributeType, Dictionary<string, object> kvps)
        {
            m_type = type;
            m_metadataObject = metadataObject;
            m_attrType = attributeType;
            m_dictKVP = kvps;
        }

        public T GetMetadataObject<T>()
        {
            return (T)m_metadataObject;
        }

        public object GetMetadataObject()
        {
            return m_metadataObject;
        }

        public Type AdditionalMetadataAttributeType
        {
            get { return m_attrType; }
        }

        public Dictionary<string, object> MetadataKVPs
        {
            get { return m_dictKVP; }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DistribProcessDetailsAttribute : DistribPluginAdditionalMetadataAttribute
    {
        private readonly _DistribProcessDetailsMetadataConcrete m_details = null;

        public DistribProcessDetailsAttribute(string name)
            : base(typeof(IDistribProcessDetailsMetadata))
        {
            m_details = new _DistribProcessDetailsMetadataConcrete();
            m_details.Name = name;
        }

        [Serializable()]
        private class _DistribProcessDetailsMetadataConcrete : IDistribProcessDetailsMetadata
        {
            public string Name { get; set; }
        }

        protected override object _provideMetadata()
        {
            return m_details;
        }
    }

    public interface IDistribProcessDetailsMetadata
    {
        string Name { get; set; }
    }
}
