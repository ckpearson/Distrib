using Distrib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distrib.Plugins
{
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class PluginAttribute : Attribute
    {
        private readonly string _name = "";
        private readonly string _description = "";
        private readonly double _version = 0.0;
        private readonly string _author = "";
        private readonly string _identifier = "";

        private readonly Type _interfaceType;

        private readonly WriteOnce<Type> _controllerType = new WriteOnce<Type>(null);

        private readonly WriteOnce<IReadOnlyList<IPluginMetadataBundle>> _lstSuppliedMetadataBundles
            = new WriteOnce<IReadOnlyList<IPluginMetadataBundle>>(null);

        public PluginAttribute(Type interfaceType,
            string name,
            string description,
            double version,
            string author,
            string identifier,
            Type controllerType = null)
        {
            _interfaceType = interfaceType;
            _name = name;
            _description = description;
            _version = version;
            _author = author;
            _identifier = identifier;
            _controllerType.Value = controllerType;
        }

        public Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }

        public double Version
        {
            get { return _version; }
        }

        public string Author
        {
            get { return _author; }
        }

        public string Identifier
        {
            get { return _identifier; }
        }

        public Type ControllerType
        {
            get
            {
                lock (_controllerType)
                {
                    return (!_controllerType.IsWritten) ? null : _controllerType.Value;
                }
            }

            set
            {
                lock (_controllerType)
                {
                    if (!_controllerType.IsWritten)
                    {
                        _controllerType.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Controller type already specified");
                    }
                }
            }
        }

        public IReadOnlyList<IPluginMetadataBundle> SuppliedMetadataBundles
        {
            get
            {
                lock (_lstSuppliedMetadataBundles)
                {
                    return (!_lstSuppliedMetadataBundles.IsWritten) ? null : _lstSuppliedMetadataBundles.Value;
                }
            }
            
            protected set
            {
                lock (_lstSuppliedMetadataBundles)
                {
                    if (!_lstSuppliedMetadataBundles.IsWritten)
                    {
                        _lstSuppliedMetadataBundles.Value = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Supplied metadata bundles already set");
                    }
                }
            }
        }
    }
}
