using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Discovery.Metadata
{
    /// <summary>
    /// Additional metadata interface for distrib process details
    /// </summary>
    public interface IDistribProcessDetailsMetadata
    {
        /// <summary>
        /// Gets or sets the name of the process
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the process
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the version of the process
        /// </summary>
        double Version { get; set; }

        /// <summary>
        /// Gets or sets the author of the process
        /// </summary>
        string Author { get; set; }
    }
}
