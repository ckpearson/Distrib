using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distrib.Plugins
{
    /// <summary>
    /// Specifies the policy for existence of a bundle of additional metadata
    /// </summary>
    public enum PluginMetadataBundleExistencePolicy
    {
        /// <summary>
        /// It's not important, any number of instances may exist
        /// </summary>
        NotImportant = 0,

        /// <summary>
        /// Only a single instance of any kind of bundle should exist for a given plugin
        /// </summary>
        SingleInstance,

        /// <summary>
        /// Only multiple instances of any kind of bundle should exist for a given plugin
        /// </summary>
        MultipleInstances,

        /// <summary>
        /// At least one instance of any kind of bundle should exist for a given plugin
        /// </summary>
        AtLeastOne,
    }
}
