using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    /// <summary>
    /// The result of performing the existence policy checks upon a plugins' additional metadata
    /// </summary>
    public enum PluginMetadataBundleExistenceCheckResult
    {
        /// <summary>
        /// Everything checked out
        /// </summary>
        Success = 0,

        /// <summary>
        /// The check failed as an existence policy was not met
        /// </summary>
        FailedExistencePolicy,

        /// <summary>
        /// The check failed as a bundle identity group had mismatched policies
        /// </summary>
        ExistencePolicyMismatch,
    }
}
