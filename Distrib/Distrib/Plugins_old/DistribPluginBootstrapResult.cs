using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins_old
{
    /// <summary>
    /// The result of performing bootstrapping of a distrib plugin's details
    /// </summary>
    public enum DistribPluginBootstrapResult
    {
        /// <summary>
        /// Everything checked out
        /// </summary>
        Success = 0,

        /// <summary>
        /// Something was wrong, but it's not clear what
        /// </summary>
        GenericFailure,


    }
}
