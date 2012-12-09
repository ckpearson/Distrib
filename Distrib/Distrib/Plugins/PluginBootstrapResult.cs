using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public enum PluginBootstrapResult
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
