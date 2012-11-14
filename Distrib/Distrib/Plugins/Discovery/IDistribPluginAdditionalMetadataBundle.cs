using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    /// <summary>
    /// Holds a bundle of additional metadata present on a plugin
    /// </summary>
    public interface IDistribPluginAdditionalMetadataBundle
    {
        /// <summary>
        /// Gets the type of the attribute that provided the additional metadata
        /// </summary>
        Type AdditionalMetadataAttributeType { get; }

        /// <summary>
        /// Gets the underlying metadata object in a given form
        /// </summary>
        /// <typeparam name="T">The interface type for the metadata</typeparam>
        /// <returns></returns>
        T GetMetadataObject<T>();

        /// <summary>
        /// Gets the concrete underlying metadata object
        /// </summary>
        /// <returns>The metadata object</returns>
        object GetMetadataObject();

        /// <summary>
        /// Gets a dictionary containing the metadata keys and their respective values.
        /// </summary>
        Dictionary<string, object> MetadataKVPs { get; }
    }
}
