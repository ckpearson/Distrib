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

namespace Distrib.Data.Transport
{
    /// <summary>
    /// Represents a data transport point
    /// </summary>
    public interface IDataTransportPoint
    {
        /// <summary>
        /// Gets the name of the point
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the data flow direction of the point
        /// </summary>
        DataTransportPointDirection Direction { get; }

        /// <summary>
        /// Gets the discovery keywords of the point
        /// </summary>
        IReadOnlyList<string> Keywords { get; }

        /// <summary>
        /// Gets or sets the value of the point
        /// </summary>
        object Value { get; set; }
    }
}
