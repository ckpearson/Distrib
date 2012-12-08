using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    public sealed class PluginMetadata : IPluginMetadata
    {
        private readonly string _name = "";
        private readonly string _description = "";
        private readonly double _version = 0.0;
        private readonly string _author = "";
        private readonly string _identifier = "";

        private readonly Type _interfaceType = null;
        private readonly WriteOnce<Type> _controllerType = new WriteOnce<Type>(null);

        public PluginMetadata(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            string identifier,
            Type controllerType)
        {
            _interfaceType = interfaceType;
            _name = name;
            _description = description;
            _version = version;
            _author = author;
            _identifier = identifier;
            if (controllerType != null) _controllerType.Value = controllerType;
        }

        public Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public Type ControllerType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public double Version
        {
            get { throw new NotImplementedException(); }
        }

        public string Author
        {
            get { throw new NotImplementedException(); }
        }

        public string Identifier
        {
            get { throw new NotImplementedException(); }
        }
    }
}
