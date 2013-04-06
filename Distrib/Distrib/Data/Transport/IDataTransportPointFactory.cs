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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Data.Transport
{
    /// <summary>
    /// Describes a factory for data transport points
    /// </summary>
    public interface IDataTransportPointFactory
    {
        /// <summary>
        /// Gets a data transport point from the decorating attribute
        /// </summary>
        /// <param name="name">The name to give to the point</param>
        /// <param name="attr">The attribute containing the details of the point</param>
        /// <param name="value">The value you wish to give to the point</param>
        /// <returns>The data transport point</returns>
        IDataTransportPoint FromAttribute(string name, DataTransportPointAttribute attr, object value);

        /// <summary>
        /// Gets a data transport point for a given property
        /// </summary>
        /// <param name="property">The property decorated with the attribute to get details from</param>
        /// <param name="onInstance">The instance to use for retrieving a value, <c>null</c> to just set values to null</param>
        /// <returns>The data transport point</returns>
        IDataTransportPoint FromAttributeOnProperty(System.Reflection.PropertyInfo property, object onInstance = null);

        /// <summary>
        /// Gets the data transport points on a given type by checking the properties and using null point values
        /// </summary>
        /// <param name="type">The type to retrieve points from</param>
        /// <returns>The collection of data transport points from the type</returns>
        IEnumerable<IDataTransportPoint> GetDataPointsFromPropertiesOnType(Type type);

        /// <summary>
        /// Gets the data transport points on a given instance by checking the properties and using their values
        /// </summary>
        /// <param name="instance">The instance to retrieve points from</param>
        /// <returns>The collection of data transport points from the instance</returns>
        IEnumerable<IDataTransportPoint> GetDataPointsFromPropertiesOnInstance(object instance);
    }
}
