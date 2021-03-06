﻿/*
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
    public interface IPluginMetadataBundle
    {
        /// <summary>
        /// Gets the underlying metadata object in a given form
        /// </summary>
        /// <typeparam name="T">The interface type for the metadata</typeparam>
        /// <returns></returns>
        T GetMetadataInstance<T>();

        /// <summary>
        /// Gets the concrete underlying metadata object
        /// </summary>
        /// <returns>The metadata object</returns>
        object GetMetadataInstance();

        /// <summary>
        /// Gets a dictionary containing the metadata keys and their respective values.
        /// </summary>
        IReadOnlyDictionary<string, object> MetadataKVPs { get; }

        /// <summary>
        /// Gets a string representing the shared identity for this type of metadata bundle
        /// </summary>
        string MetadataBundleIdentity { get; }

        /// <summary>
        /// Gets the policy for instance existence for bundles of this type (shared identity)
        /// </summary>
        PluginMetadataBundleExistencePolicy MetadataInstanceExistencePolicy { get; }
    }
}
