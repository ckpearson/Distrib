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
