/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using Distrib.Plugins;
using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    [Serializable()]
    public sealed class ProcessMetadataObject : Plugins.PluginAdditionalMetadataObject, 
        IProcessMetadata
    {
        public const string BundleIdentity  = "DistribProcessMetadata";
        public ProcessMetadataObject(
            string name,
            string description,
            double version,
            string author)
            : base(typeof(IProcessMetadata),
            BundleIdentity,
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


        public bool Match(IProcessMetadata metadata)
        {
            return CChain<bool>
                .If(() => this.Name == metadata.Name, true, false)
                .ThenIf(() => this.Description == metadata.Description, true)
                .ThenIf(() => this.Version == metadata.Version, true)
                .ThenIf(() => this.Author == metadata.Author, true)
                .Result;
        }
    }
}
