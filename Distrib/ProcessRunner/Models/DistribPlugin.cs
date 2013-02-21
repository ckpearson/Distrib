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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessRunner.Models
{
    public sealed class DistribPlugin
    {
        private IPluginDescriptor _pluginDescriptor;

        public DistribPlugin(IPluginDescriptor descriptor)
        {
            _pluginDescriptor = descriptor;
        }

        public string TypeName
        {
            get { return _pluginDescriptor.PluginTypeName; }
        }

        public bool IsUsable
        {
            get
            {
                return _pluginDescriptor.IsUsable;
            }
        }

        public Distrib.Plugins.PluginExclusionReason ExclusionReason
        {
            get
            {
                return _pluginDescriptor.ExclusionReason;
            }
        }

        public string PluginName
        {
            get
            {
                return _pluginDescriptor.Metadata.Name;
            }
        }

        public string PluginDescription
        {
            get
            {
                return _pluginDescriptor.Metadata.Description;
            }
        }

        public string PluginAuthor
        {
            get
            {
                return _pluginDescriptor.Metadata.Author;
            }
        }

        public string PluginVersion
        {
            get
            {
                return string.Format("{0:0.0}", _pluginDescriptor.Metadata.Version);
            }
        }

        public IPluginDescriptor RawDescriptor
        {
            get
            {
                return _pluginDescriptor;
            }
        }

        public string PluginIdentifier
        {
            get
            {
                return _pluginDescriptor.Metadata.Identifier;
            }
        }

        public string PluginInterface
        {
            get
            {
                return _pluginDescriptor.Metadata.InterfaceType.Namespace + "." +
                    _pluginDescriptor.Metadata.InterfaceType.Name;
            }
        }

        public string PluginController
        {
            get
            {
                return _pluginDescriptor.Metadata.ControllerType.Namespace + "." +
                    _pluginDescriptor.Metadata.ControllerType.Name;
            }
        }

        public bool HasAdditionalMetadata
        {
            get
            {
                return _pluginDescriptor.AdditionalMetadataBundles != null && _pluginDescriptor.AdditionalMetadataBundles.Count > 0;
            }
        }

        public IReadOnlyList<IPluginMetadataBundle> AdditionalMetadata
        {
            get
            {
                return _pluginDescriptor.AdditionalMetadataBundles;
            }
        }
    }
}
