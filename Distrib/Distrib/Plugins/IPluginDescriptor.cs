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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    public interface IPluginDescriptor
    {
        string PluginTypeName { get; }

        string AssemblyPath { get; }

        IPluginMetadata Metadata { get; }

        IReadOnlyList<IPluginMetadataBundle> AdditionalMetadataBundles { get; }

        void SetAdditionalMetadata(IEnumerable<IPluginMetadataBundle> additionalMetadataBundles);

        bool IsUsable { get; }

        PluginExclusionReason ExclusionReason { get; }

        object ExclusionTag { get; }

        void MarkAsUsable();

        void MarkAsUnusable(PluginExclusionReason exclusionReason, object tag = null);

        bool UsabilitySet { get; }

        /// <summary>
        /// Determines whether a given <see cref="IPluginDescriptor"/> describes the same fundamental
        /// plugin as the current one (no check into usability / metadata)
        /// </summary>
        /// <param name="descriptor">The descriptor to check against</param>
        /// <returns><c>True</c> if it is the same plugin, <c>False</c> otherwise</returns>
        bool Match(IPluginDescriptor descriptor);
    }
}
