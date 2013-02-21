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

namespace Distrib.Plugins
{
    /// <summary>
    /// Specifies the policy for existence of a bundle of additional metadata
    /// </summary>
    public enum PluginMetadataBundleExistencePolicy
    {
        /// <summary>
        /// It's not important, any number of instances may exist
        /// </summary>
        NotImportant = 0,

        /// <summary>
        /// Only a single instance of any kind of bundle should exist for a given plugin
        /// </summary>
        SingleInstance,

        /// <summary>
        /// Only multiple instances of any kind of bundle should exist for a given plugin
        /// </summary>
        MultipleInstances,
    }
}
