using Distrib.IOC;
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
    /// Factory for data transport points
    /// </summary>
    public sealed class DataTransportPointFactory :
        IDataTransportPointFactory
    {
        private readonly IIOC _ioc;

        public DataTransportPointFactory(IIOC ioc)
        {
            _ioc = ioc;
        }

        public IDataTransportPoint FromAttribute(string name, DataTransportPointAttribute attr, object value)
        {
            if (string.IsNullOrEmpty(name)) throw Ex.ArgNull(() => name);
            if (attr == null) throw Ex.ArgNull(() => attr);

            return _ioc.Get<IDataTransportPoint>(new[]
            {
                new IOCConstructorArgument(null, name),
                new IOCConstructorArgument(null, attr.Direction),
                new IOCConstructorArgument(null, attr.Keywords),
                new IOCConstructorArgument("value", value),
            });
        }

        public IDataTransportPoint FromAttributeOnProperty(System.Reflection.PropertyInfo property, object onInstance = null)
        {
            if (property == null) throw Ex.ArgNull(() => property);

            if (!Attribute.IsDefined(property, typeof(DataTransportPointAttribute)))
            {
                throw Ex.Arg(() => property, "Property isn't decorated with the data transport point attribute");
            }

            return FromAttribute(property.Name,
                (DataTransportPointAttribute)Attribute.GetCustomAttribute(property, typeof(DataTransportPointAttribute)),
                onInstance != null ? property.GetValue(onInstance) : null);
        }


        public IEnumerable<IDataTransportPoint> GetDataPointsFromPropertiesOnType(Type type)
        {
            if (type == null) throw Ex.ArgNull(() => type);
            if (!type.IsClass) throw Ex.Arg(() => type, "Type must be a class");

            foreach (var dataPointProp in type.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(DataTransportPointAttribute))))
            {
                yield return FromAttributeOnProperty(dataPointProp, null);
            }
        }


        public IEnumerable<IDataTransportPoint> GetDataPointsFromPropertiesOnInstance(object instance)
        {
            if (instance == null) throw Ex.ArgNull(() => instance);
            if (!instance.GetType().IsClass) throw Ex.Arg(() => instance, "Instance type must be a class");

            foreach (var dataPointProp in instance.GetType().GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(DataTransportPointAttribute))))
            {
                yield return FromAttributeOnProperty(dataPointProp, instance);
            }
        }
    }
}
