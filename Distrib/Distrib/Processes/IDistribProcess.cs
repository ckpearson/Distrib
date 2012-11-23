using Distrib.Plugins.Discovery.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Core interface for defining a Distrib-enabled process
    /// </summary>
    public interface IDistribProcess
    {
        string SayHello();
    }
}
