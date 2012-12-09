using Distrib.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes.Discovery.Metadata
{
    [Serializable()]
    public sealed class ProcessMetadataObject : Plugins.PluginAdditionalMetadataObject, IDistribProcessDetailsMetadata
    {
        public ProcessMetadataObject(
            string name,
            string description,
            double version,
            string author)
            : base(typeof(IDistribProcessDetailsMetadata),
            "{959D436B-FBDF-4210-A80A-F3DACC357FD6}",
            PluginMetadataBundleExistencePolicy.SingleInstance)
        {
            this.Name = name;
            this.Description = description;
            this.Version = version;
            this.Author = author;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Version { get; set; }
        public string Author { get; set; }

        protected override object _provideMetadata()
        {
            return this;
        }
    }
}
