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
    internal static class ProcessJobFieldFactory
    {
        /// <summary>
        /// Create a non-generic field
        /// </summary>
        /// <param name="type">The field type</param>
        /// <param name="name">The field name</param>
        /// <param name="mode">The field mode</param>
        /// <returns>The field</returns>
        public static IProcessJobField CreateField(Type type, string name, FieldMode mode)
        {
            return new ProcessJobField(type, name, mode);
        }

        /// <summary>
        /// Creates a generic field
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        /// <param name="name">The field name</param>
        /// <param name="mode">The field mode</param>
        /// <returns>The field</returns>
        public static IProcessJobField<T> CreateField<T>(string name, FieldMode mode)
        {
            return new ProcessJobField<T>(name, mode);
        }
    }
}
