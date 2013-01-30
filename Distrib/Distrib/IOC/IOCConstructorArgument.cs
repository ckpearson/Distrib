using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.IOC
{
    /// <summary>
    /// Represents a constructor argument for use by the IOC container
    /// </summary>
    public sealed class IOCConstructorArgument
    {
        private readonly string _argName;
        private readonly object _value;

        /// <summary>
        /// Instantiates a new constructor argument
        /// </summary>
        /// <param name="name">The name of the constructor argument</param>
        /// <param name="value">The value of the constructor argument</param>
        public IOCConstructorArgument(string name, object value)
        {
            _argName = name;
            _value = value;
        }

        /// <summary>
        /// Gets the name of the constructor argument
        /// </summary>
        public string Name { get { return _argName; } }

        /// <summary>
        /// Gets the value of the constructor argument
        /// </summary>
        public object Value { get { return _value; } }
    }
}
