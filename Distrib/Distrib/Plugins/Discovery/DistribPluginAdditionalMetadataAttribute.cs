using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins.Discovery
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DistribPluginAdditionalMetadataAttribute : Attribute
    {

    }

    public sealed class DistribProcessDetails : DistribPluginAdditionalMetadataAttribute
    {

    }
}
