using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginMetadataBundle
    {
        ///// <summary>
        ///// Gets the type of the attribute that provided the additional metadata
        ///// </summary>
        //Type AdditionalMetadataAttributeType { get; }
#warning Investigate whether the additional metadata attribute type property is required

        /// <summary>
        /// Gets the underlying metadata object in a given form
        /// </summary>
        /// <typeparam name="T">The interface type for the metadata</typeparam>
        /// <returns></returns>
        T GetMetadataInstance<T>();

        /// <summary>
        /// Gets the concrete underlying metadata object
        /// </summary>
        /// <returns>The metadata object</returns>
        object GetMetadataInstance();

        /// <summary>
        /// Gets a dictionary containing the metadata keys and their respective values.
        /// </summary>
        IReadOnlyDictionary<string, object> MetadataKVPs { get; }

        /// <summary>
        /// Gets a string representing the shared identity for this type of metadata bundle
        /// </summary>
        string MetadataBundleIdentity { get; }

        /// <summary>
        /// Gets the policy for instance existence for bundles of this type (shared identity)
        /// </summary>
        PluginMetadataBundleExistencePolicy MetadataInstanceExistencePolicy { get; }
    }
}
