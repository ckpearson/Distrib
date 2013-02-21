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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Separation
{
    [DebuggerDisplay("'{_assemblyQualName}'")]
    [Serializable()]
    public sealed class SerType
    {
        private readonly string _typeName;
        private readonly string _assemblyName;
        private readonly string _assemblyQualName;
        private readonly string _assemblyLocation;

        public SerType(Type type)
        {
            _typeName = type.FullName;
            _assemblyName = type.Assembly.FullName;
            _assemblyQualName = type.AssemblyQualifiedName;
            _assemblyLocation = type.Assembly.Location;
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public string AssemblyQualifiedName
        {
            get { return _assemblyQualName; }
        }

        public string AssemblyLocation
        {
            get { return _assemblyLocation; }
        }
    }
}
