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

namespace Distrib.Processes
{
    public static class ProcessJobFieldsGeneratorService
    {
        public static IEnumerable<IProcessJobDefinitionField> GetFieldsFromInterface(Type interfaceType, FieldMode mode)
        {
            //var foundFields =
            //                (_inputInterfaceType.GetProperties()
            //                    .Where(p => p.CanRead && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable))
            //                    .Select(p => ProcessJobFieldFactory.CreateDefinitionField(p.PropertyType, p.Name, FieldMode.Input))
            //                .Concat(
            //                    _outputInterfaceType.GetProperties()
            //                    .Where(p => (p.CanRead && p.CanWrite) && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
            //                    .Select(p => ProcessJobFieldFactory.CreateDefinitionField(p.PropertyType, p.Name, FieldMode.Output))).ToList();

            if (interfaceType == null) throw Ex.ArgNull(() => interfaceType);
            if (!interfaceType.IsInterface) throw Ex.Arg(() => interfaceType, "Interface type must be for an interface");

            try
            {
                return interfaceType.GetProperties()
                    .Where(p => p.CanRead && (p.PropertyType.IsClass || p.PropertyType.IsValueType) && p.PropertyType.IsSerializable)
                    .Select(p => ProcessJobFieldFactory.CreateDefinitionField(p.PropertyType, p.Name, mode));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to get definition fields from the given interface", ex);
            }
        }

        public static IEnumerable<IProcessJobDefinitionField> GetFieldsFromInterface<T>(FieldMode mode)
        {
            return ProcessJobFieldsGeneratorService.GetFieldsFromInterface(typeof(T), mode);
        }
    }
}
