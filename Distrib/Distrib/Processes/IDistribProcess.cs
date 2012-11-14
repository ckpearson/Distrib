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


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DistribProcessDetailsAttribute : DistribPluginAdditionalMetadataAttribute
    {
        private readonly _DistribProcessDetailsMetadataConcrete m_details = null;

        public DistribProcessDetailsAttribute(
            string name,
            string description,
            double version,
            string author)
            : base(typeof(IDistribProcessDetailsMetadata))
        {
            m_details = new _DistribProcessDetailsMetadataConcrete();
            m_details.Name = name;
        }

        [Serializable()]
        private class _DistribProcessDetailsMetadataConcrete : IDistribProcessDetailsMetadata
        {
            public string Name { get; set; }
        }

        protected override object _provideMetadata()
        {
            return m_details;
        }
    }

    public interface IDistribProcessDetailsMetadata
    {
        string Name { get; set; }
    }
}
