using Distrib.Plugins.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Discovery.Metadata
{
    /// <summary>
    /// Attribute for providing process details via additional metadata system
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DistribProcessDetailsAttribute : DistribPluginAdditionalMetadataAttribute
    {
        private readonly _DistribProcessDetailsMetadataConcrete m_details = null;

        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="name">The name of the process</param>
        /// <param name="description">The description of the process</param>
        /// <param name="version">The version of the process</param>
        /// <param name="author">The author of the process</param>
        public DistribProcessDetailsAttribute(
            string name,
            string description,
            double version,
            string author)
            : base(typeof(IDistribProcessDetailsMetadata),
                "{959D436B-FBDF-4210-A80A-F3DACC357FD6}",
                AdditionalPluginMetadataIdentityExistencePolicy.SingleInstance)
        {
            m_details = new _DistribProcessDetailsMetadataConcrete();
            m_details.Name = name;
            m_details.Description = description;
            m_details.Version = version;
            m_details.Author = author;
        }

        /// <summary>
        /// Provides the concrete bundle object holding the metadata for this attribute
        /// </summary>
        [Serializable()]
        private class _DistribProcessDetailsMetadataConcrete : IDistribProcessDetailsMetadata
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double Version { get; set; }
            public string Author { get; set; }
        }

        /// <summary>
        /// Provides the metadata object
        /// </summary>
        /// <returns></returns>
        protected override object _provideMetadata()
        {
            return m_details;
        }
    }
}
