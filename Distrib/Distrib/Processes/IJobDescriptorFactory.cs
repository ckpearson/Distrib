using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Factory that creates job descriptors
    /// </summary>
    public interface IJobDescriptorFactory
    {
        /// <summary>
        /// Create a job descriptor
        /// </summary>
        /// <param name="definition">The job definition to create the descriptor from</param>
        /// <returns>The job descriptor</returns>
        IJobDescriptor Create(IJobDefinition definition);
    }
}
