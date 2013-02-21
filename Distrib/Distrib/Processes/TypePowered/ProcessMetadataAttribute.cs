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

namespace Distrib.Processes.TypePowered
{
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ProcessMetadataAttribute : TypePoweredMetadataAttribute
    {
        public ProcessMetadataAttribute(
            string name,
            string description,
            double version,
            string author)
        {
            base.MetadataObject = new ProcessMetadataObject(
                name,
                description,
                version,
                author);
        }
    }

    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class TypePoweredMetadataAttribute : Attribute
    {
        public object MetadataObject { get; protected set; }
    }
}
