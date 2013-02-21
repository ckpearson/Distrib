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
