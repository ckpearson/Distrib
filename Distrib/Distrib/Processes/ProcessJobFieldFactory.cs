using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Processes
{
    /// <summary>
    /// Factory for creating process job fields
    /// </summary>
    public static class ProcessJobFieldFactory
    {
        /// <summary>
        /// Create a non-generic field
        /// </summary>
        /// <param name="type">The field type</param>
        /// <param name="name">The field name</param>
        /// <param name="mode">The field mode</param>
        /// <returns>The field</returns>
        public static IProcessJobDefinitionField CreateDefinitionField(Type type, string name, FieldMode mode)
        {
            return new ProcessJobFieldDefinition(type, name, mode);
        }

        /// <summary>
        /// Creates a generic field
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        /// <param name="name">The field name</param>
        /// <param name="mode">The field mode</param>
        /// <returns>The field</returns>
        public static IProcessJobDefinitionField<T> CreateDefinitionField<T>(string name, FieldMode mode)
        {
            return new ProcessJobFieldDefinition<T>(name, mode);
        }

        public static IProcessJobValueField CreateValueField(IProcessJobDefinitionField definition)
        {
            return new ProcessJobFieldValue(definition);
        }

        public static IProcessJobValueField<T> CreateValueField<T>(IProcessJobDefinitionField<T> definition)
        {
            return new ProcessJobFieldValue<T>(definition);
        }
    }
}
